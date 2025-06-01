using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniversityBooking.BookingBoard.Dtos;
using UniversityBooking.Bookings;
using UniversityBooking.Days;
using UniversityBooking.Rooms;
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

        public BookingBoardAppService(
            IRepository<Room, Guid> roomRepository,
            IRepository<Day, Guid> dayRepository,
            IRepository<TimeSlot, Guid> timeSlotRepository,
            IRepository<Booking, Guid> bookingRepository)
        {
            _roomRepository = roomRepository;
            _dayRepository = dayRepository;
            _timeSlotRepository = timeSlotRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingBoardDto> GetCurrentWeekAsync(Guid? roomId = null)
        {
            return await GetForDateAsync(DateTime.Today, roomId);
        }

        public async Task<BookingBoardDto> GetForDateAsync(DateTime date, Guid? roomId = null)
        {
            // Get all days
            var days = await _dayRepository.GetListAsync();

            // Create weekly calendar
            var weeklyCalendar = CreateWeeklyCalendar(date, days);

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
                .Where(b => b.Status == BookingStatus.Active)
                .WhereIf(roomId.HasValue, b => b.RoomId == roomId.Value)
                .ToListAsync();

            return bookings;
        }

        private WeeklyCalendar CreateWeeklyCalendar(DateTime referenceDate, List<Day> workingDays)
        {
            return new WeeklyCalendar(referenceDate, workingDays);
        }
    }
}
