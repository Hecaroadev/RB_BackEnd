// CreateUpdateTimeSlotDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityBooking.TimeSlots.Dtos
{
    public class CreateUpdateTimeSlotDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        [Required]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        public TimeSpan EndTime { get; set; }
        
        public bool IsActive { get; set; }
    }
}