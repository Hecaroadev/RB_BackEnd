// CreateBookingRequestDto.cs
using System;
using System.ComponentModel.DataAnnotations;
using UniversityBooking.Rooms;

namespace UniversityBooking.BookingRequests.Dtos
{
    public class CreateBookingRequestDto
    {
        public RoomCategory Category { get; set; }
        public Guid? RoomId { get; set; } // Optional - will be assigned by admin or selected by authenticated user

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
        public string InstructorName { get; set; }
        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        // Capacity planning - especially for labs
        [Range(0, 1000)]
        public int NumberOfStudents { get; set; }
        public bool IsRecurring { get; set; }
        public int RecurringWeeks { get; set; }
        public SoftwareTool RequiredTools { get; set; }

        // New field for anonymous users
        public string? AnonymousUserEmail { get; set; } // Email for anonymous users to track their requests
    }
}
