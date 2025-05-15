using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace UniversityBooking.BookingBoard.Dtos
{
    public class WeeklyCalendarDto : EntityDto
    {
        public DateTime WeekStartDate { get; set; }
        public DateTime WeekEndDate { get; set; }
        public int WeekNumber { get; set; }
        public string DisplayRange { get; set; }
        public List<CalendarDayItemDto> Days { get; set; }
        
        public WeeklyCalendarDto()
        {
            Days = new List<CalendarDayItemDto>();
        }
    }
}