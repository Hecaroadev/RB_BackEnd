// BookingRequest.cs
using System;
using UniversityBooking.Days;
using UniversityBooking.Rooms;
using UniversityBooking.Semesters;
using UniversityBooking.TimeSlots;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace UniversityBooking.BookingRequests
{
    public class BookingRequest : FullAuditedAggregateRoot<Guid>
    {
        public Guid RoomId { get; private set; }
        public Guid TimeSlotId { get; private set; }
        public Guid DayId { get; private set; }
        public Guid SemesterId { get; private set; }
        public string RequestedBy { get; private set; } // Username or email of requestor
        public Guid RequestedById { get; private set; } // User ID of requestor
        public DateTime RequestDate { get; private set; }
        public DateTime? RequestedDate { get; private set; } // Specific date for which the booking is requested
        public string Purpose { get; private set; }
        public BookingRequestStatus Status { get; private set; }
        public string RejectionReason { get; private set; } = string.Empty; // Default to empty string
        public DateTime? ProcessedDate { get; private set; }
        public string ProcessedBy { get; private set; }
        public Guid? ProcessedById { get; private set; }

        public virtual Room Room { get; private set; }
        public virtual TimeSlot TimeSlot { get; private set; }
        public virtual Day Day { get; private set; }
        public virtual Semester Semester { get; private set; }
        public virtual IdentityUser RequestedByUser { get; private set; }
        public virtual IdentityUser ProcessedByUser { get;  set; }

        protected BookingRequest()
        {
        }

        public BookingRequest(
            Guid id,
            Guid roomId,
            Guid timeSlotId,
            Guid dayId,
            Guid semesterId,
            Guid requestedById,
            string requestedBy,
            string purpose,
            IdentityUser requestedByUser,
            DateTime? requestedDate = null
        ) : base(id)
        {
            RoomId = roomId;
            TimeSlotId = timeSlotId;
            DayId = dayId;
            SemesterId = semesterId;
            RequestedById = requestedById;
            RequestedBy = requestedBy;
            Purpose = purpose;
            RequestDate = DateTime.Now;
            RequestedDate = requestedDate;
            Status = BookingRequestStatus.Pending;
            ProcessedBy = requestedBy;
            ProcessedByUser = requestedByUser;
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
    }

    public enum BookingRequestStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }
}
