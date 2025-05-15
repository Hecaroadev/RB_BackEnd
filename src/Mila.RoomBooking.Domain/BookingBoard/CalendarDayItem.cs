using System;
using Volo.Abp.Domain.Entities;

namespace UniversityBooking.BookingBoard
{
    public class CalendarDayItem : Entity
    {
        public DateTime Date { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsWeekend { get; private set; }
        public bool IsToday { get; private set; }
        public bool IsInSemester { get; private set; }
        public bool IsWorkingDay { get; private set; }
        public Guid? DayId { get; private set; }
        
        protected CalendarDayItem()
        {
        }
        
        public CalendarDayItem(DateTime date, Guid? dayId, bool isInSemester, bool isWorkingDay)
        {
            Date = date.Date;
            DayOfWeek = date.DayOfWeek;
            DisplayName = date.ToString("yyyy-MM-dd"); // ISO date format
            
            // In most Middle Eastern countries, weekend is Friday and Saturday
            IsWeekend = DayOfWeek == System.DayOfWeek.Friday || DayOfWeek == System.DayOfWeek.Saturday;
            IsToday = Date.Date == DateTime.Today;
            
            IsInSemester = isInSemester;
            IsWorkingDay = isWorkingDay;
            DayId = dayId;
        }
        
        public override object[] GetKeys()
        {
            return new object[] { Date };
        }
    }
}