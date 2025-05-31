// RoomBookingManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversityBooking.BookingRequests;
using UniversityBooking.Bookings;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using Microsoft.EntityFrameworkCore;
using UniversityBooking.Days;
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
        private readonly ICurrentUser _currentUser;

        public RoomBookingManager(
            IRepository<Room, Guid> roomRepository,
            IRepository<BookingRequest, Guid> bookingRequestRepository,
            IRepository<Booking, Guid> bookingRepository)
        {
            _roomRepository = roomRepository;
            _bookingRequestRepository = bookingRequestRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<bool> IsRoomAvailableAsync(
            Guid? roomId,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            // If roomId is null or empty, we're checking if any room is available
            if (!roomId.HasValue || roomId.Value == Guid.Empty)
            {
                return true; // Room will be assigned by admin during approval
            }

            // Check if room exists and is active
            var room = await _roomRepository.GetAsync(roomId.Value);
            if (room == null || !room.IsActive)
            {
                return false;
            }


            // Validate time range
            if (startTime >= endTime)
            {
                throw new ArgumentException("End time must be after start time.");
            }


            // Get all active bookings for this room on this date
            var bookingsQuery = await _bookingRepository.GetQueryableAsync();
            var existingBookings = await bookingsQuery
                .Where(b => b.RoomId == roomId &&
                           b.BookingDate != null &&
                           b.BookingDate.Value.Date == bookingDate.Date &&
                           b.Status == BookingStatus.Active)
                .ToListAsync();

            // Check if any existing booking overlaps with the requested time range
            foreach (var booking in existingBookings)
            {
                // If we have a booking with a time range that overlaps with our requested time range
                if (booking.StartTime < endTime && booking.EndTime > startTime)
                {
                    return false;
                }
            }

            // Check pending requests for the same time range
            var pendingRequestsQuery = await _bookingRequestRepository.GetQueryableAsync();
            var pendingRequests = await pendingRequestsQuery
                .Where(br => br.RoomId == roomId &&
                            br.BookingDate.Date == bookingDate.Date &&
                            br.Status == BookingRequestStatus.Pending)
                .ToListAsync();

            // Check if any pending request overlaps with the requested time range
            foreach (var request in pendingRequests)
            {
                // If we have a pending request with a time range that overlaps with our requested time range
                if (request.StartTime < endTime && request.EndTime > startTime)
                {
                    return false;
                }
            }

            return true;
        }

        [UnitOfWork]
        public async Task<BookingRequest> CreateBookingRequestAsync(
            Guid roomId,
            Guid requestedById,
            string requestedBy,
            string purpose,
            IdentityUser requestedByUser,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            DateTime? requestedDate = null
            )
        {
            // Validate availability
            var isAvailable = await IsRoomAvailableAsync(
                roomId,
                bookingDate,
                startTime,
                endTime);

            if (!isAvailable)
            {
                throw new RoomNotAvailableException("The room is not available for the selected time range.");
            }

            // Create booking request
            var bookingRequest = new BookingRequest(
              GuidGenerator.Create(),
              roomId,
              requestedById,
              requestedBy,
              purpose,
              requestedByUser,
              bookingDate,
              startTime,
              endTime
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

            // Only check room availability if a room has been assigned
            if (bookingRequest.RoomId != Guid.Empty)
            {
                // Check if the room is still available
                bool isAvailable = await IsRoomAvailableAsync(
                    bookingRequest.RoomId,
                    bookingRequest.BookingDate,
                    bookingRequest.StartTime,
                    bookingRequest.EndTime);

                if (!isAvailable)
                {
                    throw new RoomNotAvailableException("The room is no longer available for the requested time.");
                }
            }
            else
            {
                throw new InvalidOperationException("A room must be assigned before a booking request can be approved.");
            }

            // Approve the request
            bookingRequest.Approve(processedById, processedBy);

            await _bookingRequestRepository.UpdateAsync(bookingRequest);

            // Create a booking from the approved request
            var booking = Booking.CreateFromRequest(bookingRequest, GuidGenerator.Create());

            booking.BookingDate = bookingRequest.BookingDate;

            // Get a DayId from the database based on the day of week if needed
            // This is now handled in the CreateFromRequest method with null DayId

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
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = await _bookingRepository.GetQueryableAsync();

            query = query.Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active);

            // Direct date filtering, without using semesters
            if (startDate.HasValue)
            {
                query = query.Where(b => b.BookingDate != null && b.BookingDate.Value.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(b => b.BookingDate != null && b.BookingDate.Value.Date <= endDate.Value.Date);
            }

            // Removed semester filtering - now using date ranges only

            return await query
                .Include(b => b.Room)
                .Include(b => b.ReservedByUser)
                .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<List<BookingRequest>> GetPendingBookingRequestsAsync()
        {
            var query = await _bookingRequestRepository.GetQueryableAsync();

            return await query
                .Where(br => br.Status == BookingRequestStatus.Pending)
                .Include(br => br.Room)
                .Include(br => br.RequestedByUser)
                .OrderBy(br => br.RequestDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get available time slots for a specific room, day, and date
        /// </summary>


        /// <summary>
        /// Find an available room matching category and requirements for the given date and time
        /// </summary>
        public async Task<Room> FindAvailableRoomAsync(
            RoomCategory category,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int requiredCapacity,
            SoftwareTool requiredTools = SoftwareTool.None)
        {
            // Get all rooms that match the category and have sufficient capacity
            var roomsQuery = await _roomRepository.GetQueryableAsync();
            roomsQuery = roomsQuery
                .Where(r => r.IsActive)
                .Where(r => r.Category == category)
                .Where(r => r.Capacity >= requiredCapacity);

            // If software tools are required, filter rooms with those tools
            if (requiredTools != SoftwareTool.None)
            {
                // Filter rooms that have all the required tools
                // This uses a bitwise check to ensure all required flags are present
                roomsQuery = roomsQuery.Where(r => (r.AvailableTools & requiredTools) == requiredTools);
            }

            // Get rooms
            var potentialRooms = await roomsQuery.ToListAsync();
            var availableRooms = new List<Room>();

            // Check each room for availability during the specified time range
            foreach (var room in potentialRooms)
            {
                bool isAvailable = await IsRoomAvailableAsync(
                    room.Id,
                    bookingDate,
                    startTime,
                    endTime);

                if (isAvailable)
                {
                    availableRooms.Add(room);
                }
            }

            // Return the first available room, prioritizing rooms with the closest capacity match
            return availableRooms
                .OrderBy(r => r.Capacity - requiredCapacity) // Prioritize closest capacity match
                .FirstOrDefault();
        }

        /// <summary>
        /// Check if any room of the specified category is available
        /// </summary>
        public async Task<bool> IsCategoryAvailableAsync(
            RoomCategory category,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int requiredCapacity = 0,
            SoftwareTool requiredTools = SoftwareTool.None)
        {
            var availableRoom = await FindAvailableRoomAsync(
                category,
                bookingDate,
                startTime,
                endTime,
                requiredCapacity,
                requiredTools);

            return availableRoom != null;
        }

        /// <summary>
        /// Enhanced booking request creation with additional fields
        /// </summary>
        [UnitOfWork]
        public async Task<BookingRequest> CreateEnhancedBookingRequestAsync(
            Guid? roomId,
            Guid? requestedById,
            string requestedBy,
            string purpose,
            IdentityUser requestedByUser,
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
            DateTime? requestedDate = null)
        {
            // Validate time range
            if (startTime == null || endTime == null)
            {
                throw new ArgumentException("Both start time and end time must be provided.");
            }

            if (startTime.Value >= endTime.Value)
            {
                throw new ArgumentException("End time must be after start time.");
            }

            // If roomId is provided, check availability
            if (roomId.HasValue)
            {
                // Check if the room is available at the requested time
                var isAvailable = await IsRoomAvailableAsync(
                    roomId.Value,
                    bookingDate,
                    startTime.Value,
                    endTime.Value);

                if (!isAvailable)
                {
                    throw new RoomNotAvailableException("The selected room is not available for the requested time range.");
                }
            }
            // Room will be assigned by admin during approval if not provided

            // Create booking request
            var bookingRequest = new BookingRequest(
                GuidGenerator.Create(),
                roomId, // Pass the nullable roomId directly
                requestedById,
                requestedBy,
                purpose,
                requestedByUser,
                bookingDate,
                startTime.Value,
                endTime.Value,
                // New parameters
                category,
                instructorName,
                subject,
                numberOfStudents,
                isRecurring,
                recurringWeeks,
                requiredTools
            );

            await _bookingRequestRepository.InsertAsync(bookingRequest);

            return bookingRequest;
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
