// BookingRequest.cs
using System;
using System.ComponentModel.DataAnnotations;
using UniversityBooking.Days;
using UniversityBooking.Rooms;
using UniversityBooking.TimeSlots;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace UniversityBooking.BookingRequests
{
    public class BookingRequest : FullAuditedAggregateRoot<Guid>
    {
        // Room and scheduling properties
        public RoomCategory? Category { get; set; }
        public Guid? RoomId { get; set; }
        public virtual Room Room { get; set; }

        public Guid DayId { get; set; }
        public virtual Day Day { get; set; }

        public Guid? TimeSlotId { get; set; }
        public virtual TimeSlot TimeSlot { get; set; }

        public Guid? SemesterId { get; set; }

        // New time range properties
        public DateTime? BookingDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        // Request details
        [Required]
        [StringLength(500)]
        public string Purpose { get; set; }

        [StringLength(100)]
        public string InstructorName { get; set; }

        [StringLength(100)]
        public string Subject { get; set; }

        // Lab-specific properties
        public int NumberOfStudents { get; set; }
        public SoftwareTool RequiredTools { get; set; }

        // Recurring properties
        public bool IsRecurring { get; set; }
        public int RecurringWeeks { get; set; }

        // Request status and processing
        public BookingRequestStatus Status { get; set; }

        public Guid RequestedById { get; set; }
        public virtual IdentityUser RequestedByUser { get; set; }

        [Required]
        [StringLength(100)]
        public string RequestedBy { get; set; }

        public DateTime RequestDate { get; set; }

        // Processing information
        public Guid? ProcessedById { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string RejectionReason { get; set; }

        protected BookingRequest()
        {
            // For EF Core
        }

        public BookingRequest(
            Guid id,
            Guid? roomId,
            Guid? timeSlotId,
            Guid dayId,
            Guid? semesterId,
            Guid requestedById,
            string requestedBy,
            string purpose,
            IdentityUser requestedByUser,
            DateTime? requestedDate = null) : base(id)
        {
            RoomId = roomId;
            TimeSlotId = timeSlotId;
            DayId = dayId;
            SemesterId = semesterId;
            RequestedById = requestedById;
            RequestedBy = requestedBy;
            Purpose = purpose;
            RequestedByUser = requestedByUser;
            RequestDate = requestedDate ?? DateTime.UtcNow;
            Status = BookingRequestStatus.Pending;

            // Initialize default values
            NumberOfStudents = 0;
            RequiredTools = SoftwareTool.None;
            IsRecurring = false;
            RecurringWeeks = 0;
        }

        // Methods for the new workflow
        public void SetCategory(RoomCategory category)
        {
            Category = category;
        }

        public void SetTimeRange(string startTime, string endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public void SetClassDetails(string instructorName, string subject)
        {
            InstructorName = instructorName;
            Subject = subject;
        }

        public void SetLabRequirements(int numberOfStudents, SoftwareTool requiredTools)
        {
            NumberOfStudents = numberOfStudents;
            RequiredTools = requiredTools;
        }

        public void SetRecurringDetails(bool isRecurring, int recurringWeeks)
        {
            IsRecurring = isRecurring;
            RecurringWeeks = recurringWeeks;
        }

        public void AssignRoom(Guid roomId)
        {
            RoomId = roomId;
        }

        public void Approve(Guid processedById, string processedBy)
        {
            if (Status != BookingRequestStatus.Pending)
            {
                throw new InvalidOperationException("Only pending requests can be approved.");
            }

            Status = BookingRequestStatus.Approved;
            ProcessedById = processedById;
            ProcessedBy = processedBy;
            ProcessedDate = DateTime.UtcNow;
        }

        public void Reject(Guid processedById, string processedBy, string rejectionReason)
        {
            if (Status != BookingRequestStatus.Pending)
            {
                throw new InvalidOperationException("Only pending requests can be rejected.");
            }

            Status = BookingRequestStatus.Rejected;
            ProcessedById = processedById;
            ProcessedBy = processedBy;
            ProcessedDate = DateTime.UtcNow;
            RejectionReason = rejectionReason;
        }

        public bool IsForTimeRange()
        {
            return !string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime);
        }

        public bool IsForTimeSlot()
        {
            return TimeSlotId.HasValue;
        }

        public string GetDisplayTimeRange()
        {
            if (IsForTimeRange())
            {
                return $"{StartTime} - {EndTime}";
            }
            else if (TimeSlot != null)
            {
                return TimeSlot.Name;
            }
            return "Time not specified";
        }

        public string GetDisplayDate()
        {
            if (BookingDate.HasValue)
            {
                return BookingDate.Value.ToString("yyyy-MM-dd");
            }
            return "Date not specified";
        }
    }


}
