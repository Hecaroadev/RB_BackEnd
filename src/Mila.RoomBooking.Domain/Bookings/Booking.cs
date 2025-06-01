// Booking.cs
using System;
using System.ComponentModel.DataAnnotations;
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
        // Room and scheduling properties
        public Guid RoomId { get; set; }
        public virtual Room Room { get; set; }

        public Guid DayId { get; set; }
        public virtual Day Day { get; set; }

        public Guid? TimeSlotId { get; set; }
        public virtual TimeSlot TimeSlot { get; set; }

        public Guid? SemesterId { get; set; }

        // New time range properties
        public DateTime BookingDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        // Booking details
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

        // Booking status and user information
        public BookingStatus Status { get; set; }

        public Guid ReservedById { get; set; }
        public virtual IdentityUser ReservedByUser { get; set; }

        [Required]
        [StringLength(100)]
        public string ReservedBy { get; set; }

        // Cancellation information
        public string CancellationReason { get; set; }
        public DateTime? CancellationDate { get; set; }

        // Original booking request reference
        public Guid? OriginalRequestId { get; set; }

        protected Booking()
        {
            // For EF Core
        }

        public Booking(
            Guid id,
            Guid roomId,
            Guid dayId,
            Guid? timeSlotId,
            Guid? semesterId,
            DateTime bookingDate,
            string startTime,
            string endTime,
            string purpose,
            Guid reservedById,
            string reservedBy,
            IdentityUser reservedByUser) : base(id)
        {
            RoomId = roomId;
            DayId = dayId;
            TimeSlotId = timeSlotId;
            SemesterId = semesterId;
            BookingDate = bookingDate;
            StartTime = startTime;
            EndTime = endTime;
            Purpose = purpose;
            ReservedById = reservedById;
            ReservedBy = reservedBy;
            ReservedByUser = reservedByUser;
            Status = BookingStatus.Active;

            // Initialize default values
            NumberOfStudents = 0;
            RequiredTools = SoftwareTool.None;
        }

        public static Booking CreateFromRequest(BookingRequest request, Guid bookingId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!request.RoomId.HasValue)
                throw new InvalidOperationException("Cannot create booking without assigned room.");

            var bookingDate = request.BookingDate ?? DateTime.Today;

            var booking = new Booking(
                bookingId,
                request.RoomId.Value,
                request.DayId,
                request.TimeSlotId,
                request.SemesterId,
                bookingDate,
                request.StartTime ?? string.Empty,
                request.EndTime ?? string.Empty,
                request.Purpose,
                request.RequestedById,
                request.RequestedBy,
                request.RequestedByUser);

            // Copy additional properties
            booking.InstructorName = request.InstructorName;
            booking.Subject = request.Subject;
            booking.NumberOfStudents = request.NumberOfStudents;
            booking.RequiredTools = request.RequiredTools;
            booking.OriginalRequestId = request.Id;

            return booking;
        }

        public static Booking CreateFromRequestWithDate(BookingRequest request, Guid bookingId, DateTime specificDate)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!request.RoomId.HasValue)
                throw new InvalidOperationException("Cannot create booking without assigned room.");

            var booking = new Booking(
                bookingId,
                request.RoomId.Value,
                request.DayId,
                request.TimeSlotId,
                request.SemesterId,
                specificDate,
                request.StartTime ?? string.Empty,
                request.EndTime ?? string.Empty,
                request.Purpose,
                request.RequestedById,
                request.RequestedBy,
                request.RequestedByUser);

            // Copy additional properties
            booking.InstructorName = request.InstructorName;
            booking.Subject = request.Subject;
            booking.NumberOfStudents = request.NumberOfStudents;
            booking.RequiredTools = request.RequiredTools;
            booking.OriginalRequestId = request.Id;

            return booking;
        }

        public void Cancel(string reason)
        {
            if (Status != BookingStatus.Active)
            {
                throw new InvalidOperationException("Only active bookings can be cancelled.");
            }

            Status = BookingStatus.Cancelled;
            CancellationReason = reason;
            CancellationDate = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (Status == BookingStatus.Cancelled)
            {
                Status = BookingStatus.Active;
                CancellationReason = null;
                CancellationDate = null;
            }
        }

        public bool IsActive => Status == BookingStatus.Active;

        public bool IsCancelled => Status == BookingStatus.Cancelled;

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
            return BookingDate.ToString("yyyy-MM-dd");
        }

        public bool ConflictsWith(string otherStartTime, string otherEndTime)
        {
            if (string.IsNullOrEmpty(StartTime) || string.IsNullOrEmpty(EndTime) ||
                string.IsNullOrEmpty(otherStartTime) || string.IsNullOrEmpty(otherEndTime))
            {
                return false;
            }

            if (TimeSpan.TryParse(StartTime, out var thisStart) &&
                TimeSpan.TryParse(EndTime, out var thisEnd) &&
                TimeSpan.TryParse(otherStartTime, out var otherStart) &&
                TimeSpan.TryParse(otherEndTime, out var otherEnd))
            {
                // Check for time overlap: starts before other ends AND ends after other starts
                return thisStart < otherEnd && thisEnd > otherStart;
            }

            return false;
        }
    }
}
