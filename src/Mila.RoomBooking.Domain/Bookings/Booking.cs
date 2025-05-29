// Booking.cs
using System;
using UniversityBooking.BookingRequests;
using UniversityBooking.Days;
using UniversityBooking.Rooms;
using UniversityBooking.TimeSlots;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace UniversityBooking.Bookings
{
    public class Booking : FullAuditedAggregateRoot<Guid>
    {
        public Guid RoomId { get; private set; }

        public Guid? BookingRequestId { get; private set; }
        public Guid ReservedById { get; private set; }
        public string ReservedBy { get; private set; }
        public string Purpose { get; private set; }
        public bool IsRecurring { get; private set; }
        public BookingStatus Status { get; private set; }
        public DateTime? BookingDate { get; set; } // Specific date for the booking
        public TimeSpan? StartTime { get; set; } // Start time of booking
        public TimeSpan? EndTime { get; set; } // End time of booking

        public virtual Room Room { get; private set; }
        public virtual BookingRequest BookingRequest { get; private set; }
        public virtual IdentityUser ReservedByUser { get; private set; }

        protected Booking()
        {
        }

        public Booking(
            Guid id,
            Guid? roomId,
            Guid reservedById,
            string reservedBy,
            string purpose,
            Guid? bookingRequestId = null,
            bool isRecurring = false,
            DateTime? bookingDate = null,
            TimeSpan? startTime = null,
            TimeSpan? endTime = null
        ) : base(id)
        {
            RoomId = roomId ?? Guid.Empty;

            BookingRequestId = bookingRequestId;
            ReservedById = reservedById;
            ReservedBy = reservedBy;
            Purpose = purpose;
            IsRecurring = isRecurring;
            BookingDate = bookingDate;
            StartTime = startTime;
            EndTime = endTime;
            Status = BookingStatus.Active;
        }

        public static Booking CreateFromRequest(BookingRequest request, Guid id)
        {
            return new Booking(
                id,
                request.RoomId,
                request.RequestedById ?? Guid.NewGuid(),
                request.RequestedBy ?? "Unknown",
                request.Purpose,
                request.Id,
                request.IsRecurring,
                request.BookingDate,
                request.StartTime,
                request.EndTime
            );
        }


        public void Cancel(string reason)
        {
            Status = BookingStatus.Cancelled;
        }
    }

    public enum BookingStatus
    {
        Active,
        Cancelled,
        Completed
    }
}
