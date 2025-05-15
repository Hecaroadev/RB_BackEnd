using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityBooking.BookingBoard.Dtos;
using UniversityBooking.Bookings;
using UniversityBooking.Days;
using UniversityBooking.Rooms;
using UniversityBooking.Semesters;
using UniversityBooking.TimeSlots;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace UniversityBooking.BookingBoard
{
    public class BookingBoardAppService : ApplicationService, IBookingBoardAppService
    {
        private readonly IRepository<Room, Guid> _roomRepository;
        private readonly IRepository<Day, Guid> _dayRepository;
        private readonly IRepository<TimeSlot, Guid> _timeSlotRepository;
        private readonly IRepository<Booking, Guid> _bookingRepository;
        private readonly IRepository<Semester, Guid> _semesterRepository;

        public BookingBoardAppService(
            IRepository<Room, Guid> roomRepository,
            IRepository<Day, Guid> dayRepository,
            IRepository<TimeSlot, Guid> timeSlotRepository,
            IRepository<Booking, Guid> bookingRepository,
            IRepository<Semester, Guid> semesterRepository)
        {
            _roomRepository = roomRepository;
            _dayRepository = dayRepository;
            _timeSlotRepository = timeSlotRepository;
            _bookingRepository = bookingRepository;
            _semesterRepository = semesterRepository;
        }

        public async Task<BookingBoardDto> GetCurrentWeekAsync(Guid? roomId = null)
        {
            return await GetForDateAsync(DateTime.Today, roomId);
        }

        public async Task<BookingBoardDto> GetForDateAsync(DateTime date, Guid? roomId = null)
        {
            // Get the current semester
            var currentSemester = await GetCurrentSemesterAsync();
            
            // Get all days
            var days = await _dayRepository.GetListAsync();
            
            // Create weekly calendar
            var weeklyCalendar = CreateWeeklyCalendar(date, days, currentSemester);
            
            // Get rooms and time slots
            var rooms = await GetRoomsAsync(roomId);
            var timeSlots = await _timeSlotRepository.GetListAsync(ts => ts.IsActive);
            
            // Get bookings for the week
            var bookings = await GetBookingsForWeekAsync(weeklyCalendar.WeekStartDate, weeklyCalendar.WeekEndDate, roomId);
            
            // Create booking board
            var bookingBoard = new BookingBoardDto
            {
                Calendar = ObjectMapper.Map<WeeklyCalendar, WeeklyCalendarDto>(weeklyCalendar),
                Rooms = ObjectMapper.Map<List<Room>, List<Rooms.Dtos.RoomDto>>(rooms),
                TimeSlots = ObjectMapper.Map<List<TimeSlot>, List<TimeSlots.Dtos.TimeSlotDto>>(timeSlots),
                Bookings = ObjectMapper.Map<List<Booking>, List<Bookings.Dtos.BookingDto>>(bookings)
            };
            
            // Add display range for better UX
            bookingBoard.Calendar.DisplayRange = $"{weeklyCalendar.WeekStartDate:MMM dd} - {weeklyCalendar.WeekEndDate:MMM dd, yyyy}";
            
            return bookingBoard;
        }

        public async Task<BookingBoardDto> GetWeekAsync(DateTime weekStartDate, Guid? roomId = null)
        {
            return await GetForDateAsync(weekStartDate, roomId);
        }

        public async Task<bool> IsRoomAvailableAsync(Guid roomId, Guid timeSlotId, DateTime date)
        {
            // Check if the date is in semester
            var currentSemester = await GetCurrentSemesterAsync();
            if (currentSemester == null || !currentSemester.IsDateInSemester(date))
            {
                return false;
            }
            
            // Get the day of week
            var dayOfWeek = date.DayOfWeek;
            
            // Get the day entity
            var day = await _dayRepository.FirstOrDefaultAsync(d => d.DayOfWeek == dayOfWeek);
            if (day == null)
            {
                return false;
            }
            
            // Check for existing bookings at this date/time
            var existingBooking = await _bookingRepository.FirstOrDefaultAsync(b => 
                b.RoomId == roomId && 
                b.TimeSlotId == timeSlotId && 
                (
                    // Check if there's a specific date booking
                    (b.BookingDate.HasValue && b.BookingDate.Value.Date == date.Date) ||
                    // Or a recurring booking for this day of week
                    (!b.BookingDate.HasValue && b.DayId == day.Id)
                ) &&
                b.Status == BookingStatus.Active
            );
            
            return existingBooking == null;
        }

        private async Task<List<Room>> GetRoomsAsync(Guid? roomId)
        {
            if (roomId.HasValue)
            {
                var room = await _roomRepository.GetAsync(roomId.Value);
                return new List<Room> { room };
            }
            
            return await _roomRepository.GetListAsync(r => r.IsActive);
        }

        private async Task<List<Booking>> GetBookingsForWeekAsync(DateTime weekStart, DateTime weekEnd, Guid? roomId)
        {
            var bookingQuery = await _bookingRepository.GetQueryableAsync();
            
            var bookings = await bookingQuery
                .Include(b => b.Room)
                .Include(b => b.TimeSlot)
                .Include(b => b.Day)
                .Where(b => b.Status == BookingStatus.Active)
                .Where(b => 
                    // Include specific date bookings within the week range
                    (b.BookingDate.HasValue && b.BookingDate.Value.Date >= weekStart.Date && b.BookingDate.Value.Date <= weekEnd.Date) ||
                    // Include recurring bookings (no specific date) for the days in this week
                    (!b.BookingDate.HasValue)
                )
                .WhereIf(roomId.HasValue, b => b.RoomId == roomId.Value)
                .ToListAsync();
                
            return bookings;
        }

        private async Task<Semester> GetCurrentSemesterAsync()
        {
            var query = await _semesterRepository.GetQueryableAsync();
            
            // Try to get the active semester first
            var activeSemester = await query
                .Where(s => s.IsActive)
                .FirstOrDefaultAsync();
                
            if (activeSemester != null)
            {
                return activeSemester;
            }
            
            // If no active semester, get the semester that contains the current date
            var today = DateTime.Today;
            var currentSemester = await query
                .Where(s => s.StartDate <= today && s.EndDate >= today)
                .FirstOrDefaultAsync();
                
            if (currentSemester != null)
            {
                return currentSemester;
            }
            
            // If no current semester, get the nearest future semester
            var futureSemester = await query
                .Where(s => s.StartDate > today)
                .OrderBy(s => s.StartDate)
                .FirstOrDefaultAsync();
                
            return futureSemester;
        }

        private WeeklyCalendar CreateWeeklyCalendar(DateTime referenceDate, List<Day> workingDays, Semester currentSemester)
        {
            return new WeeklyCalendar(referenceDate, workingDays, currentSemester);
        }
    }
}