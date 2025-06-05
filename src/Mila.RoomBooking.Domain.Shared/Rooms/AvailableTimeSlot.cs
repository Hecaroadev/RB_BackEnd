using System;

namespace UniversityBooking.Rooms
{
    /// <summary>
    /// Represents an available time slot for booking
    /// </summary>
    public class AvailableTimeSlot
    {
        /// <summary>
        /// Start time of the available slot
        /// </summary>
        public TimeSpan StartTime { get; set; }
        
        /// <summary>
        /// End time of the available slot
        /// </summary>
        public TimeSpan EndTime { get; set; }
        
        /// <summary>
        /// Duration of the time slot in minutes
        /// </summary>
        public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;
        
        /// <summary>
        /// Formatted time range (e.g., "9:00 AM - 10:00 AM")
        /// </summary>
        public string FormattedTimeRange => $"{StartTime.ToString(@"hh\:mm tt")} - {EndTime.ToString(@"hh\:mm tt")}";
        
        public AvailableTimeSlot(TimeSpan startTime, TimeSpan endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}