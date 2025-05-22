using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mila.RoomBooking.Controllers;
using UniversityBooking.Bookings;
using UniversityBooking.Bookings.Dtos;
using Volo.Abp;

namespace Mila.RoomBooking.HttpApi.Controllers
{
    [RemoteService]
    [Route("api/app/booking")]
    public class BookingController : RoomBookingController
    {
        private readonly IBookingAppService _bookingAppService;

        public BookingController(IBookingAppService bookingAppService)
        {
            _bookingAppService = bookingAppService;
        }

        [HttpGet("by-room-and-date")]
        public async Task<List<BookingDto>> GetByRoomAndDateAsync(Guid? roomId, DateTime? date)
        {
            return await _bookingAppService.GetByRoomAndDateAsync(roomId, date);
        }
    }
}