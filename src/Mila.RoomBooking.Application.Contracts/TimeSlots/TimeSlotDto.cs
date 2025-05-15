// TimeSlotDto.cs
using System;
using Volo.Abp.Application.Dtos;

namespace UniversityBooking.TimeSlots.Dtos
{
    public class TimeSlotDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }
        
        // Formatted times for display
        public string FormattedStartTime => StartTime.ToString(@"hh\:mm");
        public string FormattedEndTime => EndTime.ToString(@"hh\:mm");
        public string DisplayName => $"{FormattedStartTime} - {FormattedEndTime}";
    }
}