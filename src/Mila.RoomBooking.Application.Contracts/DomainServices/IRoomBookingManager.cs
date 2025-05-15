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
            Guid roomId,
            Guid timeSlotId,
            Guid dayId,
            Guid semesterId);

        Task<BookingRequest> CreateBookingRequestAsync(
            Guid roomId,
            Guid timeSlotId,
            Guid dayId,
            Guid semesterId,
            Guid requestedById,
            string requestedBy,
            string purpose,
            IdentityUser identityUser,
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
            Guid? semesterId = null,
            DateTime? startDate = null,
            DateTime? endDate = null);

        Task<List<BookingRequest>> GetPendingBookingRequestsAsync();
    }
}
