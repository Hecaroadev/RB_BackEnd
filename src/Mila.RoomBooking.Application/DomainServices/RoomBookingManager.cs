// RoomBookingManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversityBooking.BookingRequests;
using UniversityBooking.Bookings;
using UniversityBooking.Semesters;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace UniversityBooking.Rooms
{
    public class RoomBookingManager : DomainService, IRoomBookingManager
    {
        private readonly IRepository<Room, Guid> _roomRepository;
        private readonly IRepository<BookingRequest, Guid> _bookingRequestRepository;
        private readonly IRepository<Booking, Guid> _bookingRepository;
        private readonly IRepository<Semester, Guid> _semesterRepository;
        private  readonly ICurrentUser _currentUser;
        public RoomBookingManager(
            IRepository<Room, Guid> roomRepository,
            IRepository<BookingRequest, Guid> bookingRequestRepository,
            IRepository<Booking, Guid> bookingRepository,
            IRepository<Semester, Guid> semesterRepository)
        {
            _roomRepository = roomRepository;
            _bookingRequestRepository = bookingRequestRepository;
            _bookingRepository = bookingRepository;
            _semesterRepository = semesterRepository;
        }

        public async Task<bool> IsRoomAvailableAsync(
            Guid roomId,
            Guid timeSlotId,
            Guid dayId,
            Guid semesterId)
        {
            // Check if room exists
            var room = await _roomRepository.GetAsync(roomId);
            if (room == null || !room.IsActive)
            {
                return false;
            }

            // Check if there's an existing active booking for the same time slot
            var existingBooking = await _bookingRepository.FirstOrDefaultAsync(b =>
                b.RoomId == roomId &&
                b.TimeSlotId == timeSlotId &&
                b.DayId == dayId &&
                b.SemesterId == semesterId &&
                b.Status == BookingStatus.Active);

            if (existingBooking != null)
            {
                return false;
            }

            // Check if there's a pending booking request for the same time slot
            var pendingRequest = await _bookingRequestRepository.FirstOrDefaultAsync(br =>
                br.RoomId == roomId &&
                br.TimeSlotId == timeSlotId &&
                br.DayId == dayId &&
                br.SemesterId == semesterId &&
                br.Status == BookingRequestStatus.Pending);

            return pendingRequest == null;
        }

        [UnitOfWork]
        public async Task<BookingRequest> CreateBookingRequestAsync(
            Guid roomId,
            Guid timeSlotId,
            Guid dayId,
            Guid semesterId,
            Guid requestedById,
            string requestedBy,
            string purpose,
            IdentityUser requestedByUser,
            DateTime? requestedDate = null)
        {
            // Validate availability
            var isAvailable = await IsRoomAvailableAsync(roomId, timeSlotId, dayId, semesterId);
            if (!isAvailable)
            {
                throw new RoomNotAvailableException("The room is not available for the selected time slot.");
            }

            // Create booking request
            var bookingRequest = new BookingRequest(
              GuidGenerator.Create(),
              roomId,
              timeSlotId,
              dayId,
              semesterId,
              requestedById,
              requestedBy,
              purpose,
              requestedByUser,
              requestedDate
            );
            await _bookingRequestRepository.InsertAsync(bookingRequest);

            return bookingRequest;
        }

        [UnitOfWork]
        public async Task<Booking> ApproveBookingRequestAsync(
            Guid bookingRequestId,
            Guid processedById,
            string processedBy)
        {
            // Get the booking request
            var bookingRequest = await _bookingRequestRepository.GetAsync(bookingRequestId);
            if (bookingRequest == null)
            {
                throw new BookingRequestNotFoundException("Booking request not found.");
            }

            if (bookingRequest.Status != BookingRequestStatus.Pending)
            {
                throw new InvalidOperationException("Only pending booking requests can be approved.");
            }

            // Check if the room is still available
            var isAvailable = await IsRoomAvailableAsync(
                bookingRequest.RoomId,
                bookingRequest.TimeSlotId,
                bookingRequest.DayId,
                bookingRequest.SemesterId);

            if (!isAvailable)
            {
                throw new RoomNotAvailableException("The room is no longer available for the requested time slot.");
            }

            // Approve the request
            bookingRequest.Approve(processedById, processedBy);

            await _bookingRequestRepository.UpdateAsync(bookingRequest);

            // Create a booking from the approved request
            var booking = Booking.CreateFromRequest(bookingRequest, GuidGenerator.Create());

            await _bookingRepository.InsertAsync(booking);

            return booking;
        }

        [UnitOfWork]
        public async Task RejectBookingRequestAsync(
            Guid bookingRequestId,
            Guid processedById,
            string processedBy,
            string rejectionReason)
        {
            var bookingRequest = await _bookingRequestRepository.GetAsync(bookingRequestId);
            if (bookingRequest == null)
            {
                throw new BookingRequestNotFoundException("Booking request not found.");
            }

            if (bookingRequest.Status != BookingRequestStatus.Pending)
            {
                throw new InvalidOperationException("Only pending booking requests can be rejected.");
            }

            bookingRequest.Reject(processedById, processedBy, rejectionReason);

            await _bookingRequestRepository.UpdateAsync(bookingRequest);
        }

        public async Task<List<Booking>> GetRoomBookingsAsync(
            Guid roomId,
            Guid? semesterId = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = await _bookingRepository.GetQueryableAsync();

            query = query.Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active);

            if (semesterId.HasValue)
            {
                query = query.Where(b => b.SemesterId == semesterId.Value);
            }

            // If dates are provided, filter by semester dates
            if (startDate.HasValue || endDate.HasValue)
            {
                var semesterQuery = await _semesterRepository.GetQueryableAsync();

                if (startDate.HasValue)
                {
                    var relevantSemesters = semesterQuery.Where(s => s.EndDate >= startDate.Value).Select(s => s.Id).ToList();
                    query = query.Where(b => relevantSemesters.Contains(b.SemesterId));
                }

                if (endDate.HasValue)
                {
                    var relevantSemesters = semesterQuery.Where(s => s.StartDate <= endDate.Value).Select(s => s.Id).ToList();
                    query = query.Where(b => relevantSemesters.Contains(b.SemesterId));
                }
            }

            return await query
                .Include(b => b.Room)
                .Include(b => b.TimeSlot)
                .Include(b => b.Day)
                .Include(b => b.Semester)
                .Include(b => b.ReservedByUser)
                .ToListAsync();
        }

        public async Task<List<BookingRequest>> GetPendingBookingRequestsAsync()
        {
            var query = await _bookingRequestRepository.GetQueryableAsync();

            return await query
                .Where(br => br.Status == BookingRequestStatus.Pending)
                .Include(br => br.Room)
                .Include(br => br.TimeSlot)
                .Include(br => br.Day)
                .Include(br => br.Semester)
                .Include(br => br.RequestedByUser)
                .OrderBy(br => br.RequestDate)
                .ToListAsync();
        }
    }

    public class RoomNotAvailableException : Exception
    {
        public RoomNotAvailableException(string message) : base(message)
        {
        }
    }

    public class BookingRequestNotFoundException : Exception
    {
        public BookingRequestNotFoundException(string message) : base(message)
        {
        }
    }
}
