// IRoomBookingManager.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityBooking.BookingRequests;
using UniversityBooking.Bookings;
using UniversityBooking.Rooms;
using Volo.Abp.Identity;

namespace UniversityBooking.Rooms
{
    public interface IRoomBookingManager
    {
        /// <summary>
        /// Check if a room is available for a specific time slot (legacy method)
        /// </summary>
        Task<bool> IsRoomAvailableAsync(Guid roomId, Guid timeSlotId, Guid dayId, Guid semesterId);

        /// <summary>
        /// Check if a room is available for a specific time range on a given date
        /// </summary>
        Task<bool> IsRoomAvailableForTimeRangeAsync(Guid roomId, Guid dayId, DateTime bookingDate, string startTime, string endTime);

        /// <summary>
        /// Create a booking request (legacy method for time slot based bookings)
        /// </summary>
        Task<BookingRequest> CreateBookingRequestAsync(
            Guid roomId,
            Guid timeSlotId,
            Guid dayId,
            Guid semesterId,
            Guid requestedById,
            string requestedBy,
            string purpose,
            IdentityUser requestedByUser,
            DateTime? requestedDate = null);

        /// <summary>
        /// Create a booking request for the new time range based workflow
        /// </summary>
        Task<BookingRequest> CreateTimeRangeBookingRequestAsync(
            RoomCategory category,
            Guid dayId,
            DateTime bookingDate,
            string startTime,
            string endTime,
            Guid requestedById,
            string requestedBy,
            string purpose,
            string instructorName,
            string subject,
            int numberOfStudents,
            SoftwareTool requiredTools,
            bool isRecurring,
            int recurringWeeks,
            IdentityUser requestedByUser);

        /// <summary>
        /// Approve a booking request (legacy method)
        /// </summary>
        Task<Booking> ApproveBookingRequestAsync(Guid bookingRequestId, Guid processedById, string processedBy);

        /// <summary>
        /// Approve a booking request and assign a room (new workflow)
        /// </summary>
        Task<Booking> ApproveBookingRequestWithRoomAssignmentAsync(
            Guid bookingRequestId,
            Guid? assignedRoomId,
            Guid processedById,
            string processedBy);

        /// <summary>
        /// Reject a booking request
        /// </summary>
        Task RejectBookingRequestAsync(Guid bookingRequestId, Guid processedById, string processedBy, string rejectionReason);

        /// <summary>
        /// Get room bookings for a specific room and date range
        /// </summary>
        Task<List<Booking>> GetRoomBookingsAsync(Guid roomId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Get all pending booking requests
        /// </summary>
        Task<List<BookingRequest>> GetPendingBookingRequestsAsync();

        /// <summary>
        /// Get available rooms for a specific time range and category
        /// </summary>
        Task<List<Room>> GetAvailableRoomsForTimeRangeAsync(
            RoomCategory category,
            Guid dayId,
            DateTime bookingDate,
            string startTime,
            string endTime);
    }
}
