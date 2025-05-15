// Day.cs
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using UniversityBooking.Calendar;
using UniversityBooking.Bookings;

namespace UniversityBooking.Days
{
    public class Day : Entity<Guid>
    {
        public string Name { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public bool IsWorkingDay { get; private set; }
        
        public virtual ICollection<Booking> Bookings { get; private set; }
        
        protected Day()
        {
            Bookings = new List<Booking>();
        }

        public Day(Guid id, string name, DayOfWeek dayOfWeek, bool isWorkingDay = true) : base(id)
        {
            Name = name;
            DayOfWeek = dayOfWeek;
            IsWorkingDay = isWorkingDay;
            Bookings = new List<Booking>();
        }
        
        public void UpdateWorkingStatus(bool isWorkingDay)
        {
            IsWorkingDay = isWorkingDay;
        }
        
        public CalendarDay ToCalendarDay(DateTime date)
        {
            var calendarDay = new CalendarDay(date);
            calendarDay.SetDayReference(this);
            return calendarDay;
        }
    }
}