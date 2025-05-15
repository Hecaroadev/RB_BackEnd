using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using UniversityBooking.Days;

namespace UniversityBooking.Calendar
{
    /// <summary>
    /// Represents a calendar week for the booking board display
    /// </summary>
    public class CalendarWeek : Entity
    {
        public DateTime WeekStartDate { get; private set; }
        public DateTime WeekEndDate { get; private set; }
        public int WeekNumber { get; private set; }
        public int Year { get; private set; }
        public List<CalendarDay> Days { get; private set; }
        
        protected CalendarWeek()
        {
            Days = new List<CalendarDay>();
        }
        
        public CalendarWeek(DateTime referenceDate)
        {
            // Get the first day of the week (Sunday)
            var diff = (int)referenceDate.DayOfWeek;
            WeekStartDate = referenceDate.AddDays(-diff).Date;
            WeekEndDate = WeekStartDate.AddDays(6).Date;
            
            // Calculate ISO week number
            WeekNumber = GetIsoWeekNumber(referenceDate);
            Year = referenceDate.Year;
            
            // Initialize days collection
            Days = new List<CalendarDay>();
            for (int i = 0; i < 7; i++)
            {
                var currentDate = WeekStartDate.AddDays(i);
                Days.Add(new CalendarDay(currentDate));
            }
        }
        
        // ISO 8601 week number calculation
        private int GetIsoWeekNumber(DateTime date)
        {
            var day = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                date = date.AddDays(3);
            }
            
            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                date, 
                System.Globalization.CalendarWeekRule.FirstFourDayWeek, 
                DayOfWeek.Monday);
        }
        
        public override object[] GetKeys()
        {
            return new object[] { WeekStartDate };
        }
    }
}