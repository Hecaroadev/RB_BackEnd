// BookingRequest.cs
using System;
using UniversityBooking.Rooms;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace UniversityBooking.BookingRequests
{
    public class BookingRequest : FullAuditedAggregateRoot<Guid>
    {
        // Basic booking information
        public Guid? RoomId { get; private set; } // Can be Guid.Empty if not assigned yet
        public string? RequestedBy { get; private set; } // Username or email of requestor
        public Guid? RequestedById { get; private set; } // User ID of requestor
        public DateTime RequestDate { get; private set; } // When the request was created
        public DateTime BookingDate { get; private set; } // The specific calendar date for the booking
        public TimeSpan StartTime { get; private set; } // Start time of booking
        public TimeSpan EndTime { get; private set; } // End time of booking
        public string Purpose { get; private set; }
        public BookingRequestStatus Status { get; private set; }
        public string? RejectionReason { get; private set; } = string.Empty; // Default to empty string
        public DateTime? ProcessedDate { get; private set; }
        public string ProcessedBy { get; private set; }
        public Guid? ProcessedById { get; private set; }

        // New fields for enhanced booking process
        public RoomCategory Category { get; private set; } // Room category for the request
        public string InstructorName { get; private set; } // Name of the instructor
        public string Subject { get; private set; } // Subject for the class/event
        public int NumberOfStudents { get; private set; } // Number of students (for capacity planning)
       // public TimeSpan StartTime { get; private set; } // Start time for custom time ranges
    //    public TimeSpan EndTime { get; private set; } // End time for custom time ranges
        public bool IsRecurring { get; private set; } // Whether this booking should repeat weekly
        public int RecurringWeeks { get; private set; } // How many weeks to repeat (if recurring)
        public SoftwareTool RequiredTools { get; private set; } // Software tools required (for labs)

        // Navigation properties
        public virtual Room Room { get; private set; }
        public virtual IdentityUser RequestedByUser { get; private set; }
        public virtual IdentityUser ProcessedByUser { get; set; }

        protected BookingRequest()
        {
        }

        public BookingRequest(
            Guid id,
            Guid? roomId,
            Guid requestedById,
            string requestedBy,
            string purpose,
            IdentityUser requestedByUser,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            // New parameters
            RoomCategory category = RoomCategory.Regular,
            string instructorName = null,
            string subject = null,
            int numberOfStudents = 0,
            bool isRecurring = false,
            int recurringWeeks = 0,
            SoftwareTool requiredTools = SoftwareTool.None
        ) : base(id)
        {
          RoomId = roomId ?? null;// Use Empty GUID if no room is assigned yet
            RequestedById = requestedById;
            RequestedBy = requestedBy;
            Purpose = purpose;
            RequestDate = DateTime.Now;
            Status = BookingRequestStatus.Pending;
            ProcessedBy = requestedBy;
            ProcessedByUser = requestedByUser;
            BookingDate = bookingDate;
            StartTime = startTime;
            EndTime = endTime;

            // Initialize new fields
            Category = category;
            InstructorName = instructorName ?? string.Empty;
            Subject = subject ?? string.Empty;
            NumberOfStudents = numberOfStudents;
            IsRecurring = isRecurring;
            RecurringWeeks = recurringWeeks;
            RequiredTools = requiredTools;
        }

        public void Approve(Guid processedById, string processedBy)
        {
            Status = BookingRequestStatus.Approved;
            ProcessedById = processedById;
            ProcessedBy = processedBy;
            ProcessedDate = DateTime.Now;
        }

        public void Reject(Guid processedById, string processedBy, string rejectionReason)
        {
            Status = BookingRequestStatus.Rejected;
            ProcessedById = processedById;
            ProcessedBy = processedBy;
            RejectionReason = rejectionReason;
            ProcessedDate = DateTime.Now;
        }

        public void Cancel()
        {
            if (Status == BookingRequestStatus.Pending)
            {
                Status = BookingRequestStatus.Cancelled;
            }
            else
            {
                throw new InvalidOperationException("Cannot cancel a request that is not pending.");
            }
        }

        public void UpdateRoom(Guid roomId)
        {
            if (Status == BookingRequestStatus.Pending)
            {
                RoomId = roomId;
            }
            else
            {
                throw new InvalidOperationException("Cannot update room for a request that is not pending.");
            }
        }
    }

    public enum BookingRequestStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }
}
