// ProcessBookingRequestDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityBooking.BookingRequests.Dtos
{
    public class ProcessBookingRequestDto
    {
        [Required]
        public Guid BookingRequestId { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        [StringLength(500)]
        public string? RejectionReason { get; set; }
        
        // Room ID for admin assignment during approval
        public Guid? RoomId { get; set; }
    }
}
