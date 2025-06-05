// BookingDto.cs
using System;
using UniversityBooking.Bookings;
using UniversityBooking.Days.Dtos;
using UniversityBooking.Rooms.Dtos;
// Removed: using UniversityBooking.Semesters.Dtos;
using UniversityBooking.TimeSlots.Dtos;
using Volo.Abp.Application.Dtos;

namespace UniversityBooking.Bookings.Dtos
{
    public class BookingDto : EntityDto<Guid>
    {
        public Guid RoomId { get; set; }
        public Guid TimeSlotId { get; set; }
        public Guid DayId { get; set; }
        // Removed: public Guid SemesterId { get; set; }
        public Guid? BookingRequestId { get; set; }
        public Guid ReservedById { get; set; }
        public string ReservedBy { get; set; }
        public string Purpose { get; set; }
        public bool IsRecurring { get; set; }
        public BookingStatus Status { get; set; }
        public string StatusString => Status.ToString();
        public DateTime? BookingDate { get; set; }
        public string FormattedBookingDate => BookingDate?.ToString("yyyy-MM-dd");
        
        // Navigation properties
        public RoomDto Room { get; set; }
        public TimeSlotDto TimeSlot { get; set; }
        public DayDto Day { get; set; }
        // Removed: public SemesterDto Semester { get; set; }
    }
}