using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UniversityBooking.Bookings;
using UniversityBooking.Days;
using UniversityBooking.Rooms;
using UniversityBooking.TimeSlots;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Mila.RoomBooking.SchaduledBookings
{
    [Authorize]
    public class SchaduledBookingAppService : ApplicationService, ISchaduledBookingAppService
    {
        private readonly IRepository<SchaduledBooking, Guid> _scheduledBookingRepository;
        private readonly IRepository<Room, Guid> _roomRepository;
        private readonly IRepository<Day, Guid> _dayRepository;
        private readonly IRepository<TimeSlot, Guid> _timeSlotRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly ICurrentUser _currentUser;

        public SchaduledBookingAppService(
            IRepository<SchaduledBooking, Guid> scheduledBookingRepository,
            IRepository<Room, Guid> roomRepository,
            IRepository<Day, Guid> dayRepository,
            IRepository<TimeSlot, Guid> timeSlotRepository,
            IRepository<IdentityUser, Guid> userRepository,
            ICurrentUser currentUser)
        {
            _scheduledBookingRepository = scheduledBookingRepository;
            _roomRepository = roomRepository;
            _dayRepository = dayRepository;
            _timeSlotRepository = timeSlotRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<WeekGridDto> GetWeekGridByRoomIdAsync(Guid roomId, DateTime weekStartDate)
        {
            // Validate room exists
            var room = await _roomRepository.GetAsync(roomId);
            if (room == null || !room.IsActive)
            {
                throw new BusinessException("RoomBooking:RoomNotFound", $"Room with ID {roomId} not found or inactive");
            }

            // Get all days ordered by day of week
            var days = await _dayRepository.GetListAsync();
            var orderedDays = days.OrderBy(d => d.DayOfWeek).ToList();

            // Get all active time slots ordered by start time
            var timeSlots = await _timeSlotRepository.GetListAsync();
            var activeTimeSlots = timeSlots.Where(ts => ts.IsActive).OrderBy(ts => ts.StartTime).ToList();

            // Get all scheduled bookings for this room
            var scheduledBookings = await _scheduledBookingRepository
                .GetQueryableAsync()
                .Result
                .Include(sb => sb.TimeRange)
                .Where(sb => sb.RoomId == roomId && sb.Status != BookingStatus.Cancelled)
                .ToListAsync();

            // Build the week grid
            var weekGrid = new WeekGridDto
            {
                RoomId = roomId,
                ScheduledRows = new List<ScheduledRowDto>()
            };

            foreach (var day in orderedDays)
            {
                var dayBookings = scheduledBookings.Where(sb => sb.DayId == day.Id).ToList();
                var scheduledRow = new ScheduledRowDto
                {
                    DayDefinitionId = day.Id,
                    DayName = day.Name,
                    TimeSlots = new List<TimeSlotStateDto>()
                };

                int order = 0;
                foreach (var timeSlot in activeTimeSlots)
                {
                    var timeSlotState = new TimeSlotStateDto
                    {
                        TimeSlotDefinitionId = timeSlot.Id,
                        StartTime = timeSlot.StartTime,
                        EndTime = timeSlot.EndTime,
                        Order = order++,
                        IsBooked = false,
                        ScheduledBookingId = null
                    };

                    // Check if this time slot is booked
                    var booking = dayBookings.FirstOrDefault(b =>
                        b.TimeRange.Any(tr => tr.Id == timeSlot.Id));

                    if (booking != null)
                    {
                        timeSlotState.IsBooked = true;
                        timeSlotState.ScheduledBookingId = booking.Id;
                        timeSlotState.InstructorName = booking.InstructorName;
                        timeSlotState.Subject = booking.Subject;
                    }

                    scheduledRow.TimeSlots.Add(timeSlotState);
                }

                weekGrid.ScheduledRows.Add(scheduledRow);
            }

            return weekGrid;
        }

        public async Task<SchaduledBookingDto> CreateScheduledBookingAsync(CreateUpdateSchaduledBookingDto input)
        {

          try
          {
     // Validate room
            var room = await _roomRepository.GetAsync(input.RoomId);
            if (!room.IsActive)
            {
                throw new BusinessException("RoomBooking:RoomNotActive", "Cannot book an inactive room");
            }

            // Validate day
            var day = await _dayRepository.GetAsync(input.DayId);

            // Validate time slots
            if (input.TimeRange == null || !input.TimeRange.Any())
            {
                throw new BusinessException("RoomBooking:NoTimeSlots", "At least one time slot must be selected");
            }

            // Validate time slots exist and are consecutive
            var timeSlotIds = input.TimeRange.Select(ts => ts.Id).ToList();
            var timeSlots = await _timeSlotRepository.GetListAsync();
            var selectedTimeSlots = timeSlots
                .Where(ts => timeSlotIds.Contains(ts.Id) && ts.IsActive)
                .OrderBy(ts => ts.StartTime)
                .ToList();

            if (selectedTimeSlots.Count != input.TimeRange.Count)
            {
                throw new BusinessException("RoomBooking:InvalidTimeSlots", "Some selected time slots are invalid or inactive");
            }

            // Validate consecutive time slots
            for (int i = 0; i < selectedTimeSlots.Count - 1; i++)
            {
                if (selectedTimeSlots[i].EndTime != selectedTimeSlots[i + 1].StartTime)
                {
                    throw new BusinessException("RoomBooking:NonConsecutiveTimeSlots", "Selected time slots must be consecutive");
                }
            }

            // Check availability
            var existingBookings = await _scheduledBookingRepository
                .GetQueryableAsync()
                .Result
                .Include(sb => sb.TimeRange)
                .Where(sb => sb.RoomId == input.RoomId &&
                            sb.DayId == input.DayId &&
                            sb.Status != BookingStatus.Cancelled)
                .ToListAsync();

            foreach (var existingBooking in existingBookings)
            {
                var hasConflict = existingBooking.TimeRange.Any(ts => timeSlotIds.Contains(ts.Id));
                if (hasConflict)
                {
                    throw new BusinessException("RoomBooking:TimeSlotConflict",
                        "One or more selected time slots are already booked");
                }
            }

            // Create the booking
            var scheduledBooking = new SchaduledBooking(
                GuidGenerator.Create(),
                input.RoomId,
                input.DayId,
                input.Purpose,
                input.InstructorName,
                input.Subject,
                BookingStatus.Confirmed,
                selectedTimeSlots
            );

            // Set the user who made the booking
            if (_currentUser.Id.HasValue)
            {
                var user = await _userRepository.GetAsync(_currentUser.Id.Value);
                scheduledBooking.ReservedByUser = user;
            }

            // Save the booking
            await _scheduledBookingRepository.InsertAsync(scheduledBooking);
            return MapToDto(scheduledBooking);

          }
          catch (Exception e)
          {
           throw new UserFriendlyException(e.Message, "194");

          }

            // Map to DTO
        }

        public async Task<bool> CheckAvailabilityAsync(AvailabilityCheckRequestDto input)
        {
            // Convert booking date to day of week
            var dayOfWeek = input.BookingDate.DayOfWeek;
            var day = await _dayRepository.FirstOrDefaultAsync(d => d.DayOfWeek == dayOfWeek);

            if (day == null)
            {
                throw new BusinessException("RoomBooking:DayNotFound", $"Day configuration for {dayOfWeek} not found");
            }

            // Get time slots that fall within the requested time range
            var timeSlots = await _timeSlotRepository.GetListAsync();
            var requestedTimeSlots = timeSlots
                .Where(ts => ts.IsActive &&
                            ts.StartTime >= input.TimeRangeStart &&
                            ts.EndTime <= input.TimeRangeEnd)
                .Select(ts => ts.Id)
                .ToList();

            if (!requestedTimeSlots.Any())
            {
                throw new BusinessException("RoomBooking:NoTimeSlotsInRange",
                    "No active time slots found in the requested time range");
            }

            // Check for conflicts
            var conflictQuery =  _scheduledBookingRepository
              .GetQueryableAsync()
              .Result
              .Include(sb => sb.TimeRange)
              .Where(sb => sb.RoomId == input.RoomId &&
                           sb.DayId == day.Id &&
                           sb.Status != BookingStatus.Cancelled);

            // Exclude current booking if editing
            if (input.ExcludeBookingId.HasValue)
            {
                conflictQuery = conflictQuery.Where(sb => sb.Id != input.ExcludeBookingId.Value);
            }

            var existingBookings = await conflictQuery.ToListAsync();

            // Check if any existing booking has overlapping time slots
            foreach (var existingBooking in existingBookings)
            {
                var hasConflict = existingBooking.TimeRange
                    .Any(ts => requestedTimeSlots.Contains(ts.Id));

                if (hasConflict)
                {
                    return false; // Not available
                }
            }

            return true; // Available
        }

        public async Task<SchaduledBookingDto> GetScheduledBookingByIdAsync(Guid sbId)
        {
            var scheduledBooking = await _scheduledBookingRepository
                .GetQueryableAsync()
                .Result
                .Include(sb => sb.Room)
                .Include(sb => sb.Day)
                .Include(sb => sb.TimeRange)
                .Include(sb => sb.ReservedByUser)
                .FirstOrDefaultAsync(sb => sb.Id == sbId);

            if (scheduledBooking == null)
            {
                throw new EntityNotFoundException(typeof(SchaduledBooking), sbId);
            }

            return MapToDto(scheduledBooking);
        }

        public async Task CancelScheduledBookingBySbIdAsync(Guid sbId)
        {
            var scheduledBooking = await _scheduledBookingRepository
                .GetQueryableAsync()
                .Result
                .Include(sb => sb.ReservedByUser)
                .FirstOrDefaultAsync(sb => sb.Id == sbId);






            // Option 1: Soft delete by changing status
            scheduledBooking.Status = BookingStatus.Cancelled;
            await _scheduledBookingRepository.UpdateAsync(scheduledBooking);

            // Option 2: Hard delete (uncomment if preferred)
            // await _scheduledBookingRepository.DeleteAsync(scheduledBooking);
        }

        #region Helper Methods

        private SchaduledBookingDto MapToDto(SchaduledBooking entity)
        {
            return new SchaduledBookingDto
            {
                Id = entity.Id,
                RoomId = entity.RoomId,
                DayId = entity.DayId,
                Purpose = entity.Purpose,
                InstructorName = entity.InstructorName,
                Subject = entity.Subject,
                Status = entity.Status,
                TimeRange = entity.TimeRange.ToList(),
                Room = entity.Room,
                Day = entity.Day,
                ReservedByUser = entity.ReservedByUser
            };
        }

        private bool IsTimeSlotInRange(TimeSlot slot, TimeSpan startTime, TimeSpan endTime)
        {
            return slot.StartTime >= startTime && slot.EndTime <= endTime;
        }

        private bool AreTimeSlotsConsecutive(List<TimeSlot> slots)
        {
            if (slots.Count <= 1) return true;

            var orderedSlots = slots.OrderBy(s => s.StartTime).ToList();
            for (int i = 0; i < orderedSlots.Count - 1; i++)
            {
                if (orderedSlots[i].EndTime != orderedSlots[i + 1].StartTime)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
