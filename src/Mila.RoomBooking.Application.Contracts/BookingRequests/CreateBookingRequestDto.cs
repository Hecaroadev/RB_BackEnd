// CreateBookingRequestDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityBooking.BookingRequests.Dtos
{
    public class CreateBookingRequestDto
    {
        [Required]
        public Guid RoomId { get; set; }
        
        [Required]
        public Guid TimeSlotId { get; set; }
        
        [Required]
        public Guid DayId { get; set; }
        
        [Required]
        public Guid SemesterId { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Purpose { get; set; }
        
        // Optional specific date for the booking
        public DateTime? RequestedDate { get; set; }
    }
}