// BookingRequestDto.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UniversityBooking.BookingRequests;
using UniversityBooking.Days.Dtos;
using UniversityBooking.Rooms;
using UniversityBooking.Rooms.Dtos;
using UniversityBooking.TimeSlots.Dtos;
using Volo.Abp.Application.Dtos;

namespace UniversityBooking.BookingRequests.Dtos
{
    public class BookingRequestDto : EntityDto<Guid>
    {
        // Basic booking information
        public Guid RoomId { get; set; }
        public Guid? TimeSlotId { get; set; }
        public Guid DayId { get; set; }
        public string RequestedBy { get; set; }
        public Guid RequestedById { get; set; }
        public DateTime RequestDate { get; set; }
        public string Purpose { get; set; }
        public BookingRequestStatus Status { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string StatusString => Status.ToString();
        public string RejectionReason { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string ProcessedBy { get; set; }
        public Guid? ProcessedById { get; set; }

        // New fields for enhanced booking process
        public RoomCategory Category { get; set; }
        public string CategoryString => Category.ToString();
        public string InstructorName { get; set; }
        public string Subject { get; set; }
        public int NumberOfStudents { get; set; }
        public bool IsRecurring { get; set; }
        public int RecurringWeeks { get; set; }
        public SoftwareTool RequiredTools { get; set; }

        // Navigation properties
        public RoomDto Room { get; set; }
        public TimeSlotDto TimeSlot { get; set; }
        public DayDto Day { get; set; }

        // Formatted date for display
        public string FormattedRequestDate => RequestDate.ToString("g");
        public string FormattedProcessedDate => ProcessedDate?.ToString("g");
        public string FormattedBookingDate => BookingDate.ToString("yyyy-MM-dd");

        // Format times as strings
        public string FormattedStartTime => StartTime.ToString(@"hh\:mm");
        public string FormattedEndTime => EndTime.ToString(@"hh\:mm");
        public string FormattedTimeRange => $"{FormattedStartTime} - {FormattedEndTime}";
        public string FormattedDayAndDate => $"{Day?.Name}, {FormattedBookingDate}";

        // Helper property to get the required tools as a list of strings
        public List<string> RequiredToolsList
        {
            get
            {
                if (RequiredTools == SoftwareTool.None)
                    return new List<string>();

                return Enum.GetValues(typeof(SoftwareTool))
                    .Cast<SoftwareTool>()
                    .Where(t => t != SoftwareTool.None && RequiredTools.HasFlag(t))
                    .Select(t => t.ToString())
                    .ToList();
            }
        }
    }
}
