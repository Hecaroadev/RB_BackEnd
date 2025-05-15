using System;
using System.Collections.Generic;
using UniversityBooking.Bookings.Dtos;
using UniversityBooking.Rooms.Dtos;
using UniversityBooking.TimeSlots.Dtos;
using Volo.Abp.Application.Dtos;

namespace UniversityBooking.BookingBoard.Dtos
{
    public class BookingBoardDto : EntityDto
    {
        public WeeklyCalendarDto Calendar { get; set; }
        public List<RoomDto> Rooms { get; set; }
        public List<TimeSlotDto> TimeSlots { get; set; }
        public List<BookingDto> Bookings { get; set; }
        
        public BookingBoardDto()
        {
            Rooms = new List<RoomDto>();
            TimeSlots = new List<TimeSlotDto>();
            Bookings = new List<BookingDto>();
        }
    }
}