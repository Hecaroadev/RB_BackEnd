using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityBooking.BookingBoard;
using UniversityBooking.BookingBoard.Dtos;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace UniversityBooking.Controllers
{
    [RemoteService]
    [Route("api/booking-board")]
    public class BookingBoardController : AbpControllerBase
    {
        private readonly IBookingBoardAppService _bookingBoardAppService;

        public BookingBoardController(IBookingBoardAppService bookingBoardAppService)
        {
            _bookingBoardAppService = bookingBoardAppService;
        }

        [HttpGet]
        [Route("current-week")]
        public async Task<BookingBoardDto> GetCurrentWeekAsync(Guid? roomId = null)
        {
            return await _bookingBoardAppService.GetCurrentWeekAsync(roomId);
        }

        [HttpGet]
        [Route("for-date")]
        public async Task<BookingBoardDto> GetForDateAsync(DateTime date, Guid? roomId = null)
        {
            return await _bookingBoardAppService.GetForDateAsync(date, roomId);
        }

        [HttpGet]
        [Route("week")]
        public async Task<BookingBoardDto> GetWeekAsync(DateTime weekStartDate, Guid? roomId = null)
        {
            return await _bookingBoardAppService.GetWeekAsync(weekStartDate, roomId);
        }

        [HttpGet]
        [Route("check-availability")]
        public async Task<bool> IsRoomAvailableAsync(Guid roomId, Guid timeSlotId, DateTime date)
        {
            return await _bookingBoardAppService.IsRoomAvailableAsync(roomId, timeSlotId, date);
        }
    }
}