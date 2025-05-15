// Booking.cs
using System;
using UniversityBooking.BookingRequests;
using UniversityBooking.Days;
using UniversityBooking.Rooms;
using UniversityBooking.Semesters;
using UniversityBooking.TimeSlots;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace UniversityBooking.Bookings
{
    public class Booking : FullAuditedAggregateRoot<Guid>
    {
        public Guid RoomId { get; private set; }
        public Guid TimeSlotId { get; private set; }
        public Guid DayId { get; private set; }
        public Guid SemesterId { get; private set; }
        public Guid? BookingRequestId { get; private set; }
        public Guid ReservedById { get; private set; }
        public string ReservedBy { get; private set; }
        public string Purpose { get; private set; }
        public bool IsRecurring { get; private set; }
        public BookingStatus Status { get; private set; }
        public DateTime? BookingDate { get; private set; } // Specific date for the booking
        
        public virtual Room Room { get; private set; }
        public virtual TimeSlot TimeSlot { get; private set; }
        public virtual Day Day { get; private set; }
        public virtual Semester Semester { get; private set; }
        public virtual BookingRequest BookingRequest { get; private set; }
        public virtual IdentityUser ReservedByUser { get; private set; }

        protected Booking()
        {
        }

        public Booking(
            Guid id,
            Guid roomId,
            Guid timeSlotId,
            Guid dayId,
            Guid semesterId,
            Guid reservedById,
            string reservedBy,
            string purpose,
            Guid? bookingRequestId = null,
            bool isRecurring = false,
            DateTime? bookingDate = null
        ) : base(id)
        {
            RoomId = roomId;
            TimeSlotId = timeSlotId;
            DayId = dayId;
            SemesterId = semesterId;
            BookingRequestId = bookingRequestId;
            ReservedById = reservedById;
            ReservedBy = reservedBy;
            Purpose = purpose;
            IsRecurring = isRecurring;
            BookingDate = bookingDate;
            Status = BookingStatus.Active;
        }

        public static Booking CreateFromRequest(BookingRequest request, Guid id)
        {
            return new Booking(
                id,
                request.RoomId,
                request.TimeSlotId,
                request.DayId,
                request.SemesterId,
                request.RequestedById,
                request.RequestedBy,
                request.Purpose,
                request.Id,
                false,
                request.RequestedDate
            );
        }
        
        public bool IsForDate(DateTime date)
        {
            // If booking has a specific date, check exact match
            if (BookingDate.HasValue)
            {
                return BookingDate.Value.Date == date.Date;
            }
            
            // Otherwise check if it's for the same day of week
            return Day?.DayOfWeek == date.DayOfWeek;
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