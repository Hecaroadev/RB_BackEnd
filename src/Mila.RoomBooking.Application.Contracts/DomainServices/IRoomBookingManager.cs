// IRoomBookingManager.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityBooking.BookingRequests;
using UniversityBooking.Bookings;
using Volo.Abp.Identity;

namespace UniversityBooking.Rooms
{
    public interface IRoomBookingManager
    {
        Task<bool> IsRoomAvailableAsync(
            Guid? roomId,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime);

        /// <summary>
        /// Find an available room matching category and requirements for the given date and time
        /// </summary>
        Task<Room> FindAvailableRoomAsync(
            RoomCategory category,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int requiredCapacity,
            SoftwareTool requiredTools = SoftwareTool.None);

        /// <summary>
        /// Check if any room of the specified category is available
        /// </summary>
        Task<bool> IsCategoryAvailableAsync(
            RoomCategory category,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int requiredCapacity = 0,
            SoftwareTool requiredTools = SoftwareTool.None);

        /// <summary>
        /// Get available time slots for a specific room, day, and date
        /// </summary>


        /// <summary>
        /// Basic booking request creation with time range
        /// </summary>
        Task<BookingRequest> CreateBookingRequestAsync(
            Guid roomId,
            Guid requestedById,
            string requestedBy,
            string purpose,
            IdentityUser identityUser,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            DateTime? requestedDate = null);

        /// <summary>
        /// Enhanced booking request creation with additional fields
        /// </summary>
        Task<BookingRequest> CreateEnhancedBookingRequestAsync(
            Guid? roomId,
            Guid? requestedById,
            string? requestedBy,
            string purpose,
            IdentityUser? identityUser,
            DateTime bookingDate,
            RoomCategory category,
            string instructorName,
            string subject,
            int numberOfStudents,
            TimeSpan? startTime,
            TimeSpan? endTime,
            bool isRecurring,
            int recurringWeeks,
            SoftwareTool requiredTools,
            DateTime? requestedDate = null);

        Task<Booking> ApproveBookingRequestAsync(
            Guid bookingRequestId,
            Guid processedById,
            string processedBy);

        Task RejectBookingRequestAsync(
            Guid bookingRequestId,
            Guid processedById,
            string processedBy,
            string rejectionReason);

        Task<List<Booking>> GetRoomBookingsAsync(
            Guid roomId,
            DateTime? startDate = null,
            DateTime? endDate = null);

        Task<List<BookingRequest>> GetPendingBookingRequestsAsync();
    }
}
