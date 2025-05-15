using System;
using UniversityBooking.Days.Dtos;
using Volo.Abp.Application.Dtos;

namespace UniversityBooking.BookingBoard.Dtos
{
    public class CalendarDayItemDto : EntityDto
    {
        public DateTime Date { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string DisplayName { get; set; }
        public string FormattedDate { get; set; }
        public bool IsWeekend { get; set; }
        public bool IsToday { get; set; }
        public bool IsInSemester { get; set; }
        public bool IsWorkingDay { get; set; }
        public Guid? DayId { get; set; }
        public DayDto Day { get; set; }
    }
}