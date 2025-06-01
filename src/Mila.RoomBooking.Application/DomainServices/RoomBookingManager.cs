// RoomBookingManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityBooking.BookingRequests;
using UniversityBooking.Bookings;
using UniversityBooking.Days;
using UniversityBooking.TimeSlots;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace UniversityBooking.Rooms
{
    public class RoomBookingManager : DomainService, IRoomBookingManager
    {
        private readonly IRepository<Room, Guid> _roomRepository;
        private readonly IRepository<BookingRequest, Guid> _bookingRequestRepository;
        private readonly IRepository<Booking, Guid> _bookingRepository;
        private readonly IRepository<Day, Guid> _dayRepository;
        private readonly IRepository<TimeSlot, Guid> _timeSlotRepository;

        public RoomBookingManager(
            IRepository<Room, Guid> roomRepository,
            IRepository<BookingRequest, Guid> bookingRequestRepository,
            IRepository<Booking, Guid> bookingRepository,
            IRepository<Day, Guid> dayRepository,
            IRepository<TimeSlot, Guid> timeSlotRepository)
        {
            _roomRepository = roomRepository;
            _bookingRequestRepository = bookingRequestRepository;
            _bookingRepository = bookingRepository;
            _dayRepository = dayRepository;
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<bool> IsRoomAvailableAsync(
            Guid roomId,
            Guid timeSlotId,
            Guid dayId,
            Guid semesterId)
        {
            // Check if room exists
            var room = await _roomRepository.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null || !room.IsActive)
            {
                return false;
            }

            // Check if there's an existing active booking for the same time slot
            var existingBooking = await _bookingRepository.FirstOrDefaultAsync(b =>
                b.RoomId == roomId &&
                b.TimeSlotId == timeSlotId &&
                b.DayId == dayId &&
                b.SemesterId == semesterId &&
                b.Status == BookingStatus.Active);

            if (existingBooking != null)
            {
                return false;
            }

            // Check if there's a pending booking request for the same time slot
            var pendingRequest = await _bookingRequestRepository.FirstOrDefaultAsync(br =>
                br.RoomId == roomId &&
                br.TimeSlotId == timeSlotId &&
                br.DayId == dayId &&
                br.SemesterId == semesterId &&
                br.Status == BookingRequestStatus.Pending);

            return pendingRequest == null;
        }

        /// <summary>
        /// Check if a room is available for a specific time range on a given date
        /// </summary>
        public async Task<bool> IsRoomAvailableForTimeRangeAsync(
            Guid roomId,
            Guid dayId,
            DateTime bookingDate,
            string startTime,
            string endTime)
        {
            // Check if room exists and is active
            var room = await _roomRepository.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null || !room.IsActive)
            {
                return false;
            }

            // Parse time ranges
            if (!TimeSpan.TryParse(startTime, out var requestStartTime) ||
                !TimeSpan.TryParse(endTime, out var requestEndTime))
            {
                return false;
            }

            // Check existing bookings for conflicts
            var bookingQuery = await _bookingRepository.GetQueryableAsync();
            var existingBookings = await bookingQuery
                .Where(b => b.RoomId == roomId &&
                           b.DayId == dayId &&
                           b.BookingDate.Date == bookingDate.Date &&
                           b.Status == BookingStatus.Active)
                .ToListAsync();

            foreach (var booking in existingBookings)
            {
                if (TimeSpan.TryParse(booking.StartTime, out var bookingStartTime) &&
                    TimeSpan.TryParse(booking.EndTime, out var bookingEndTime))
                {
                    // Check for time overlap
                    if (requestStartTime < bookingEndTime && requestEndTime > bookingStartTime)
                    {
                        return false;
                    }
                }
            }

            // Check pending booking requests for conflicts
            var requestQuery = await _bookingRequestRepository.GetQueryableAsync();
            var pendingRequests = await requestQuery
                .Where(br => br.RoomId == roomId &&
                            br.DayId == dayId &&
                            br.BookingDate.HasValue &&
                            br.BookingDate.Value.Date == bookingDate.Date &&
                            br.Status == BookingRequestStatus.Pending)
                .ToListAsync();

            foreach (var request in pendingRequests)
            {
                if (!string.IsNullOrEmpty(request.StartTime) && !string.IsNullOrEmpty(request.EndTime) &&
                    TimeSpan.TryParse(request.StartTime, out var reqStartTime) &&
                    TimeSpan.TryParse(request.EndTime, out var reqEndTime))
                {
                    // Check for time overlap
                    if (requestStartTime < reqEndTime && requestEndTime > reqStartTime)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [UnitOfWork]
        public async Task<BookingRequest> CreateBookingRequestAsync(
            Guid roomId,
            Guid timeSlotId,
            Guid dayId,
            Guid semesterId,
            Guid requestedById,
            string requestedBy,
            string purpose,
            IdentityUser requestedByUser,
            DateTime? requestedDate = null)
        {
            // Validate availability (for legacy time slot based bookings)
            var isAvailable = await IsRoomAvailableAsync(roomId, timeSlotId, dayId, semesterId);
            if (!isAvailable)
            {
                throw new RoomNotAvailableException("The room is not available for the selected time slot.");
            }

            // Create booking request
            var bookingRequest = new BookingRequest(
                GuidGenerator.Create(),
                roomId,
                timeSlotId,
                dayId,
                semesterId,
                requestedById,
                requestedBy,
                purpose,
                requestedByUser,
                requestedDate
            );

            await _bookingRequestRepository.InsertAsync(bookingRequest);
            return bookingRequest;
        }

        /// <summary>
        /// Create a booking request for the new time range based workflow
        /// </summary>
        [UnitOfWork]
        public async Task<BookingRequest> CreateTimeRangeBookingRequestAsync(
            RoomCategory category,
            Guid dayId,
            DateTime bookingDate,
            string startTime,
            string endTime,
            Guid requestedById,
            string requestedBy,
            string purpose,
            string instructorName,
            string subject,
            int numberOfStudents,
            SoftwareTool requiredTools,
            bool isRecurring,
            int recurringWeeks,
            IdentityUser requestedByUser)
        {
            // Validate day exists
            var day = await _dayRepository.GetAsync(dayId);
            if (day == null)
            {
                throw new UserFriendlyException("Invalid day selected.");
            }

            // Validate time format
            if (!TimeSpan.TryParse(startTime, out var startTimeSpan) ||
                !TimeSpan.TryParse(endTime, out var endTimeSpan))
            {
                throw new UserFriendlyException("Invalid time format.");
            }

            if (endTimeSpan <= startTimeSpan)
            {
                throw new UserFriendlyException("End time must be after start time.");
            }

            // Create booking request (room will be assigned during approval)
            var bookingRequest = new BookingRequest(
                GuidGenerator.Create(),
                null, // Room will be assigned during approval
                null, // TimeSlot not used in new workflow
                dayId,
                null, // Semester not required for the new workflow
                requestedById,
                requestedBy,
                purpose,
                requestedByUser,
                bookingDate
            );

            // Set additional properties for the new workflow
            bookingRequest.SetCategory(category);
            bookingRequest.SetTimeRange(startTime, endTime);
            bookingRequest.SetClassDetails(instructorName, subject);
            bookingRequest.SetLabRequirements(numberOfStudents, requiredTools);
            bookingRequest.SetRecurringDetails(isRecurring, recurringWeeks);

            await _bookingRequestRepository.InsertAsync(bookingRequest);
            return bookingRequest;
        }

        [UnitOfWork]
        public async Task<Booking> ApproveBookingRequestAsync(
            Guid bookingRequestId,
            Guid processedById,
            string processedBy)
        {
            // Get the booking request with error handling
            var bookingRequest = await _bookingRequestRepository.FirstOrDefaultAsync(br => br.Id == bookingRequestId);
            if (bookingRequest == null)
            {
                throw new BookingRequestNotFoundException($"Booking request with ID {bookingRequestId} not found.");
            }

            if (bookingRequest.Status != BookingRequestStatus.Pending)
            {
                throw new InvalidOperationException("Only pending booking requests can be approved.");
            }

            // For legacy bookings with room and time slot already assigned
            if (bookingRequest.RoomId.HasValue && bookingRequest.TimeSlotId.HasValue && bookingRequest.SemesterId.HasValue)
            {
                // Check if the room is still available
                var isAvailable = await IsRoomAvailableAsync(
                    bookingRequest.RoomId.Value,
                    bookingRequest.TimeSlotId.Value,
                    bookingRequest.DayId,
                    bookingRequest.SemesterId.Value);

                if (!isAvailable)
                {
                    throw new RoomNotAvailableException("The room is no longer available for the requested time slot.");
                }
            }

            // Approve the request
            bookingRequest.Approve(processedById, processedBy);
            await _bookingRequestRepository.UpdateAsync(bookingRequest);

            // Create a booking from the approved request
            var booking = Booking.CreateFromRequest(bookingRequest, GuidGenerator.Create());
            await _bookingRepository.InsertAsync(booking);

            return booking;
        }

        public Task<Booking> ApproveBookingRequestWithRoomAssignmentAsync(Guid bookingRequestId, Guid? assignedRoomId, Guid processedById,
          string processedBy)
        {
          throw new NotImplementedException();
        }

        /// <summary>
        /// Approve a booking request and assign a room (new workflow)
        /// </summary>
        [UnitOfWork]
        public async Task<Booking> ApproveBookingRequestWithRoomAssignmentAsync(
            Guid bookingRequestId,
            Guid assignedRoomId,
            Guid processedById,
            string processedBy)
        {
            // Get the booking request with error handling
            var bookingRequest = await _bookingRequestRepository.FirstOrDefaultAsync(br => br.Id == bookingRequestId);
            if (bookingRequest == null)
            {
                throw new BookingRequestNotFoundException($"Booking request with ID {bookingRequestId} not found.");
            }

            if (bookingRequest.Status != BookingRequestStatus.Pending)
            {
                throw new InvalidOperationException("Only pending booking requests can be approved.");
            }

            // Validate the assigned room exists and is active
            var assignedRoom = await _roomRepository.FirstOrDefaultAsync(r => r.Id == assignedRoomId);
            if (assignedRoom == null || !assignedRoom.IsActive)
            {
                throw new UserFriendlyException("The selected room is not available or does not exist.");
            }

            // Check if the assigned room matches the requested category
            if (bookingRequest.Category.HasValue && assignedRoom.Category != bookingRequest.Category.Value)
            {
                throw new UserFriendlyException($"The selected room is not suitable for the requested category ({bookingRequest.Category.Value}).");
            }

            // For time range bookings, check availability
            if (!string.IsNullOrEmpty(bookingRequest.StartTime) &&
                !string.IsNullOrEmpty(bookingRequest.EndTime) &&
                bookingRequest.BookingDate.HasValue)
            {
                var isAvailable = await IsRoomAvailableForTimeRangeAsync(
                    assignedRoomId,
                    bookingRequest.DayId,
                    bookingRequest.BookingDate.Value,
                    bookingRequest.StartTime,
                    bookingRequest.EndTime);

                if (!isAvailable)
                {
                    throw new RoomNotAvailableException("The assigned room is not available for the requested time range.");
                }
            }

            // Assign the room to the booking request
            bookingRequest.AssignRoom(assignedRoomId);

            // Approve the request
            bookingRequest.Approve(processedById, processedBy);
            await _bookingRequestRepository.UpdateAsync(bookingRequest);

            // Create booking(s) from the approved request
            var bookings = new List<Booking>();

            if (bookingRequest.IsRecurring && bookingRequest.RecurringWeeks > 0)
            {
                // Create multiple bookings for recurring requests
                for (int week = 0; week < bookingRequest.RecurringWeeks; week++)
                {
                    var bookingDate = bookingRequest.BookingDate.Value.AddDays(week * 7);

                    // Skip if the date is in the past
                    if (bookingDate.Date < DateTime.Today)
                        continue;

                    var booking = Booking.CreateFromRequestWithDate(bookingRequest, GuidGenerator.Create(), bookingDate);
                    await _bookingRepository.InsertAsync(booking);
                    bookings.Add(booking);
                }
            }
            else
            {
                // Create single booking
                var booking = Booking.CreateFromRequest(bookingRequest, GuidGenerator.Create());
                await _bookingRepository.InsertAsync(booking);
                bookings.Add(booking);
            }

            return bookings.FirstOrDefault();
        }

        [UnitOfWork]
        public async Task RejectBookingRequestAsync(
            Guid bookingRequestId,
            Guid processedById,
            string processedBy,
            string rejectionReason)
        {
            var bookingRequest = await _bookingRequestRepository.FirstOrDefaultAsync(br => br.Id == bookingRequestId);
            if (bookingRequest == null)
            {
                throw new BookingRequestNotFoundException($"Booking request with ID {bookingRequestId} not found.");
            }

            if (bookingRequest.Status != BookingRequestStatus.Pending)
            {
                throw new InvalidOperationException("Only pending booking requests can be rejected.");
            }

            bookingRequest.Reject(processedById, processedBy, rejectionReason);
            await _bookingRequestRepository.UpdateAsync(bookingRequest);
        }

        public async Task<List<Booking>> GetRoomBookingsAsync(
            Guid roomId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var queryable = await _bookingRepository.GetQueryableAsync();

            var query = queryable.Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active);

            if (startDate.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date <= endDate.Value.Date);
            }

            return await query
                .Include(b => b.Room)
                .Include(b => b.TimeSlot)
                .Include(b => b.Day)
                .Include(b => b.ReservedByUser)
                .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<List<BookingRequest>> GetPendingBookingRequestsAsync()
        {
            var queryable = await _bookingRequestRepository.GetQueryableAsync();

            return await queryable
                .Where(br => br.Status == BookingRequestStatus.Pending)
                .Include(br => br.Room)
                .Include(br => br.TimeSlot)
                .Include(br => br.Day)
                .Include(br => br.RequestedByUser)
                .OrderBy(br => br.RequestDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get available rooms for a specific time range and category
        /// </summary>
        public async Task<List<Room>> GetAvailableRoomsForTimeRangeAsync(
            RoomCategory category,
            Guid dayId,
            DateTime bookingDate,
            string startTime,
            string endTime)
        {
            var queryable = await _roomRepository.GetQueryableAsync();

            // Filter by category and active status
            var query = queryable.Where(r => r.IsActive && r.Category == category);

            var allRooms = await query.ToListAsync();
            var availableRooms = new List<Room>();

            foreach (var room in allRooms)
            {
                var isAvailable = await IsRoomAvailableForTimeRangeAsync(
                    room.Id, dayId, bookingDate, startTime, endTime);

                if (isAvailable)
                {
                    availableRooms.Add(room);
                }
            }

            return availableRooms.OrderBy(r => r.Building).ThenBy(r => r.Number).ToList();
        }
    }

    public class RoomNotAvailableException : Exception
    {
        public RoomNotAvailableException(string message) : base(message)
        {
        }
    }

    public class BookingRequestNotFoundException : Exception
    {
        public BookingRequestNotFoundException(string message) : base(message)
        {
        }
    }
}
