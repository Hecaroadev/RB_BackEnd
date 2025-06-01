// CreateBookingRequestDto.cs
using System;
using System.ComponentModel.DataAnnotations;
using UniversityBooking.Rooms;

namespace UniversityBooking.BookingRequests.Dtos
{
    public class CreateBookingRequestDto
    {
        // Room category is required for the new workflow
        [Required]
        public RoomCategory? Category { get; set; }

        // Room ID is optional - will be assigned by admin during approval
        public Guid? RoomId { get; set; }

        // Day ID is required
        [Required]
        public Guid? DayId { get; set; }

        // Time slot ID is optional in the new workflow
        public Guid? TimeSlotId { get; set; }

        // Semester ID is optional in the new workflow
        public Guid? SemesterId { get; set; }

        // Booking date is required
        [Required]
        public DateTime? BookingDate { get; set; }

        // Time range is required for the new workflow
        [Required]
        [StringLength(5)] // HH:mm format
        public string StartTime { get; set; }

        [Required]
        [StringLength(5)] // HH:mm format
        public string EndTime { get; set; }

        // Class/Event details
        [Required]
        [StringLength(500)]
        public string Purpose { get; set; }

        [Required]
        [StringLength(100)]
        public string InstructorName { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        // Lab-specific requirements
        public int? NumberOfStudents { get; set; }

        public SoftwareTool? RequiredTools { get; set; }

        // Recurring options
        public bool IsRecurring { get; set; } = false;

        [Range(1, 16)]
        public int? RecurringWeeks { get; set; }
    }
}
