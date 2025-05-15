// BookingRequestDto.cs
using System;
using UniversityBooking.BookingRequests;
using UniversityBooking.Days.Dtos;
using UniversityBooking.Rooms.Dtos;
using UniversityBooking.Semesters.Dtos;
using UniversityBooking.TimeSlots.Dtos;
using Volo.Abp.Application.Dtos;

namespace UniversityBooking.BookingRequests.Dtos
{
    public class BookingRequestDto : EntityDto<Guid>
    {
        public Guid RoomId { get; set; }
        public Guid TimeSlotId { get; set; }
        public Guid DayId { get; set; }
        public Guid SemesterId { get; set; }
        public string RequestedBy { get; set; }
        public Guid RequestedById { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string Purpose { get; set; }
        public BookingRequestStatus Status { get; set; }
        public string StatusString => Status.ToString();
        public string RejectionReason { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string ProcessedBy { get; set; }
        public Guid? ProcessedById { get; set; }
        
        // Navigation properties
        public RoomDto Room { get; set; }
        public TimeSlotDto TimeSlot { get; set; }
        public DayDto Day { get; set; }
        public SemesterDto Semester { get; set; }
        
        // Formatted date for display
        public string FormattedRequestDate => RequestDate.ToString("g");
        public string FormattedRequestedDate => RequestedDate?.ToString("yyyy-MM-dd");
        public string FormattedProcessedDate => ProcessedDate?.ToString("g");
    }
}