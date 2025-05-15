using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using UniversityBooking.Days;

namespace UniversityBooking.Calendar
{
    /// <summary>
    /// Represents a specific calendar day with date information for booking
    /// </summary>
    public class CalendarDay : Entity
    {
        public DateTime Date { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsWeekend { get; private set; }
        public bool IsToday { get; private set; }
        public Guid? DayId { get; private set; }
        public bool IsInSemester { get; set; }
        
        public Day Day { get; set; }
        
        protected CalendarDay()
        {
        }
        
        public CalendarDay(DateTime date)
        {
            Date = date.Date;
            DayOfWeek = date.DayOfWeek;
            DisplayName = date.ToString("ddd, MMM d"); // e.g. "Mon, May 13"
            
            IsWeekend = DayOfWeek == System.DayOfWeek.Friday || DayOfWeek == System.DayOfWeek.Saturday;
            IsToday = Date.Date == DateTime.Today;
            
            // DayId will be set later when matched with the appropriate Day entity
            DayId = null;
            IsInSemester = false;
        }
        
        public void SetDayReference(Day day)
        {
            if (day != null && day.DayOfWeek == DayOfWeek)
            {
                DayId = day.Id;
                Day = day;
            }
        }
        
        public void MarkAsInSemester(bool isInSemester)
        {
            IsInSemester = isInSemester;
        }
        
        public override object[] GetKeys()
        {
            return new object[] { Date };
        }
    }
}