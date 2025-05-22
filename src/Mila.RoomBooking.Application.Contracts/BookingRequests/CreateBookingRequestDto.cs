// CreateBookingRequestDto.cs
using System;
using System.ComponentModel.DataAnnotations;
using UniversityBooking.Rooms;

namespace UniversityBooking.BookingRequests.Dtos
{
    public class CreateBookingRequestDto
    {
        // Basic required fields - note that RoomId can be null for Lab category
        public Guid? RoomId { get; set; }

        // TimeSlot is now optional since we're using explicit time ranges
        public Guid TimeSlotId { get; set; }

        [Required]
        public Guid DayId { get; set; }

        [Required]
        [StringLength(500)]
        public string Purpose { get; set; }

        // Date and time information
        [Required]
        public DateTime BookingDate { get; set; }

        // Explicit time range (required)
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        // New fields for enhanced booking process
        [Required]
        public RoomCategory Category { get; set; } = RoomCategory.Regular;

        // Instructor information - required for all categories
        [Required]
        [StringLength(100)]
        public string InstructorName { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        // Capacity planning - especially for labs
        [Range(0, 1000)]
        public int NumberOfStudents { get; set; }

        // Recurring booking information
        public bool IsRecurring { get; set; } = false;

        [Range(0, 52)]
        public int RecurringWeeks { get; set; } = 0;

        // Software requirements for Lab category
        public SoftwareTool RequiredTools { get; set; } = SoftwareTool.None;
    }
}
