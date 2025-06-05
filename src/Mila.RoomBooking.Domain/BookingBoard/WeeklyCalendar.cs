using System;
using System.Collections.Generic;
using System.Linq;
using UniversityBooking.Days;
using Volo.Abp.Domain.Entities;

namespace UniversityBooking.BookingBoard
{
    /// <summary>
    /// Represents a calendar week view for the booking board
    /// </summary>
    public class WeeklyCalendar : Entity
    {
        public DateTime WeekStartDate { get; private set; }
        public DateTime WeekEndDate { get; private set; }
        public int WeekNumber { get; private set; }
        public List<CalendarDayItem> Days { get; private set; }
        
        protected WeeklyCalendar()
        {
            Days = new List<CalendarDayItem>();
        }
        
        public WeeklyCalendar(DateTime referenceDate, IEnumerable<Day> workingDays)
        {
            // Calculate the start of the week (Sunday)
            var dayOfWeek = (int)referenceDate.DayOfWeek;
            WeekStartDate = referenceDate.AddDays(-dayOfWeek).Date;
            WeekEndDate = WeekStartDate.AddDays(6).Date;
            
            // Calculate week number (ISO-8601 standard)
            WeekNumber = GetIsoWeekNumber(referenceDate);
            
            // Create day items for the week
            Days = new List<CalendarDayItem>();
            for (int i = 0; i < 7; i++)
            {
                var date = WeekStartDate.AddDays(i);
                var matchingDay = workingDays.FirstOrDefault(d => d.DayOfWeek == date.DayOfWeek);
                
                // All dates are considered valid now (no semester check)
                bool isAvailable = true;
                
                Days.Add(new CalendarDayItem(
                    date,
                    matchingDay?.Id,
                    isAvailable,
                    matchingDay != null
                ));
            }
        }
        
        private int GetIsoWeekNumber(DateTime date)
        {
            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                date,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }
        
        public WeeklyCalendar GetNextWeek(IEnumerable<Day> workingDays)
        {
            return new WeeklyCalendar(WeekEndDate.AddDays(1), workingDays);
        }
        
        public WeeklyCalendar GetPreviousWeek(IEnumerable<Day> workingDays)
        {
            return new WeeklyCalendar(WeekStartDate.AddDays(-1), workingDays);
        }
        
        public override object[] GetKeys()
        {
            return new object[] { WeekStartDate };
        }
    }
}