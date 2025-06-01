// BookingRequestAppService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityBooking.BookingRequests.Dtos;
using UniversityBooking.Bookings;
using UniversityBooking.Rooms;
using UniversityBooking.Rooms.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace UniversityBooking.BookingRequests
{
    public class BookingRequestAppService : ApplicationService, IBookingRequestAppService
    {
        private readonly IRepository<BookingRequest, Guid> _repository;
        private readonly IRoomBookingManager _roomBookingManager;
        private readonly IRepository<Booking, Guid> _bookingRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IRepository<Room, Guid> _roomRepository; // Add this


        public BookingRequestAppService(
            IRepository<BookingRequest, Guid> repository,
            IRoomBookingManager roomBookingManager,
            IRepository<Booking, Guid> bookingRepository,
            ICurrentUser currentUser,

            IIdentityUserRepository userRepository, IRepository<Room, Guid> roomRepository)
        {
            _repository = repository;
            _roomBookingManager = roomBookingManager;
            _bookingRepository = bookingRepository;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        public async Task<PagedResultDto<BookingRequestDto>> GetPendingRequestsAsync(PagedAndSortedResultRequestDto input)
        {
            var query = await _repository.GetQueryableAsync();

            query = query
                .Where(br => br.Status == BookingRequestStatus.Pending)
                .Include(br => br.Room)
                .Include(br => br.RequestedByUser);

            var totalCount = await query.CountAsync();

            query = query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                query = query.OrderBy(br => br.RequestDate);
            }
            else
            {
                query = query.OrderBy(br => br.RequestDate);
            }

            var bookingRequests = await query.ToListAsync();
            var bookingRequestDtos = ObjectMapper.Map<List<BookingRequest>, List<BookingRequestDto>>(bookingRequests);

            return new PagedResultDto<BookingRequestDto>(totalCount, bookingRequestDtos);
        }

        public async Task<PagedResultDto<BookingRequestDto>> GetAllRequestsAsync( )
        {
          var query = await _repository.GetQueryableAsync();

          query = query
            .Where(br => br.Status != BookingRequestStatus.Pending)
            .Include(br => br.Room)
            .Include(br => br.RequestedByUser);

          var totalCount = await query.CountAsync();

          query = query
            .Take(1000);

            query = query.OrderBy(br => br.RequestDate);



          var bookingRequests = await query.ToListAsync();
          var bookingRequestDtos = ObjectMapper.Map<List<BookingRequest>, List<BookingRequestDto>>(bookingRequests);

          return new PagedResultDto<BookingRequestDto>(totalCount, bookingRequestDtos);
        }

        public async Task<PagedResultDto<BookingRequestDto>> GetApprovedRequestsAsync(PagedAndSortedResultRequestDto input)
        {
          var query = await _repository.GetQueryableAsync();

          query = query
            .Where(br => br.Status == BookingRequestStatus.Approved)
            .Include(br => br.Room)
            .Include(br => br.RequestedByUser);

          var totalCount = await query.CountAsync();

          query = query
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

          if (!string.IsNullOrWhiteSpace(input.Sorting))
          {
            query = query.OrderBy(br => br.RequestDate);
          }
          else
          {
            query = query.OrderBy(br => br.RequestDate);
          }

          var bookingRequests = await query.ToListAsync();
          var bookingRequestDtos = ObjectMapper.Map<List<BookingRequest>, List<BookingRequestDto>>(bookingRequests);

          return new PagedResultDto<BookingRequestDto>(totalCount, bookingRequestDtos);
        }

        public async Task<BookingRequestDto> GetAsync(Guid id)
        {
            var bookingRequest = await _repository.GetAsync(id);

            // Allow anonymous users to view their own requests (by ID)
            // Admin users can view any request
            if (_currentUser.IsAuthenticated &&
                bookingRequest.RequestedById != _currentUser.Id &&
                !await AuthorizationService.IsGrantedAsync("UniversityBooking.BookingRequest.Manage"))
            {
                throw new UnauthorizedAccessException("You don't have permission to view this booking request.");
            }

            return ObjectMapper.Map<BookingRequest, BookingRequestDto>(bookingRequest);
        }

        [AllowAnonymous] // Allow anonymous access to create booking requests
        public async Task<BookingRequestDto> CreateAsync(CreateBookingRequestDto input)
        {
            try
            {
                // Validate input
                if (input == null)
                    throw new UserFriendlyException("Invalid booking request data.");

                // Validate required fields for all categories
                if (string.IsNullOrWhiteSpace(input.InstructorName))
                    throw new UserFriendlyException("Instructor name is required.");

                if (string.IsNullOrWhiteSpace(input.Subject))
                    throw new UserFriendlyException("Subject is required.");

                // Validate category-specific requirements
                if (input.Category == RoomCategory.Lab)
                {
                    if (input.NumberOfStudents <= 0)
                        throw new UserFriendlyException("Number of students is required for lab bookings.");
                }

                // Validate time range
                if (input.StartTime >= input.EndTime)
                {
                    throw new UserFriendlyException("End time must be after start time.");
                }

                // Determine if user is authenticated
                bool isAuthenticated = _currentUser.IsAuthenticated && _currentUser.Id.HasValue;
                IdentityUser currentUser = null;
                string requestedBy = "Anonymous User";
                Guid? requestedById = null;

                if (isAuthenticated)
                {
                    currentUser = await _userRepository.GetAsync(_currentUser.Id.Value);
                    requestedBy = _currentUser.UserName ?? currentUser.Email ?? "Unknown User";
                    requestedById = _currentUser.Id.Value;
                  //  input.AnonymousUserEmail = _currentUser.Email; // Store email for anonymous users if available


                }
                else if (!string.IsNullOrWhiteSpace(input.AnonymousUserEmail))
                {
                    // For anonymous users, use their email as identifier
                    requestedBy = input.AnonymousUserEmail;
                }

                BookingRequest bookingRequest;

                // If user is authenticated and room is selected, auto-approve the request
                if (isAuthenticated && input.RoomId.HasValue)
                {
                    // Create a booking request
                    bookingRequest = await _roomBookingManager.CreateEnhancedBookingRequestAsync(
                        input.RoomId,
                        requestedById ?? null,
                        requestedBy,
                        input.Purpose,
                        currentUser,
                        input.BookingDate,
                        input.Category,
                        input.InstructorName,
                        input.Subject,
                        input.NumberOfStudents,
                        input.StartTime,
                        input.EndTime,
                        input.IsRecurring,
                        input.RecurringWeeks,
                        input.RequiredTools,
                        DateTime.Now
                    );

                    // Ensure the request is saved to the database
                    await CurrentUnitOfWork.SaveChangesAsync();

                    try
                    {
                        // Auto-approve the request
                        await _roomBookingManager.ApproveBookingRequestAsync(
                            bookingRequest.Id,
                            requestedById.Value,
                            requestedBy
                        );

                        // Reload to get updated status
                        bookingRequest = await _repository.GetAsync(bookingRequest.Id);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning($"Failed to auto-approve booking request {bookingRequest.Id}: {ex.Message}");
                        // Continue with the non-approved request
                    }
                }
                else
                {
                    // Create a pending booking request (anonymous user or no room selected)
                    bookingRequest = await _roomBookingManager.CreateEnhancedBookingRequestAsync(
                        input.RoomId,
                        requestedById  ?? null,
                        requestedBy,
                        input.Purpose,
                        currentUser,
                        input.BookingDate,
                        input.Category,
                        input.InstructorName,
                        input.Subject,
                        input.NumberOfStudents,
                        input.StartTime,
                        input.EndTime,
                        input.IsRecurring,
                        input.RecurringWeeks,
                        input.RequiredTools,
                        DateTime.Now
                    );
                }

                return ObjectMapper.Map<BookingRequest, BookingRequestDto>(bookingRequest);
            }
            catch (Exception e)
            {
                if (e is UserFriendlyException)
                {
                    throw;
                }
                else if (e is RoomNotAvailableException)
                {
                    throw new UserFriendlyException(
                        "No room is available that meets your requirements. " +
                        "Please try a different time or adjust your requirements.",
                        details: e.Message);
                }
                else if (e is ArgumentNullException)
                {
                    throw new UserFriendlyException(
                        "Missing required information. " +
                        "Please fill in all required fields.",
                        details: e.Message);
                }
                else if (e is Volo.Abp.Domain.Entities.EntityNotFoundException)
                {
                    Logger.LogException(e);
                    throw new UserFriendlyException(
                        "Unable to process the booking request due to a data synchronization issue. " +
                        "Please try again in a few moments.",
                        details: $"Error ID: {Guid.NewGuid()} - Entity not found error: {e.Message}");
                }
                else
                {
                    var errorId = Guid.NewGuid();
                    Logger.LogError(e, "Booking request error {ErrorId}: {ErrorMessage}", errorId, e.Message);
                    throw new UserFriendlyException(
                        "An error occurred while processing your booking request. " +
                        "Please try again or contact support if the problem persists.",
                        details: $"Error ID: {errorId}");
                }
            }
        }

        [AllowAnonymous]
        public async Task<bool> IsCategoryAvailableAsync(
            RoomCategory category,
            Guid dayId,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int requiredCapacity = 0,
            SoftwareTool requiredTools = SoftwareTool.None)
        {
            try
            {
                return await _roomBookingManager.IsCategoryAvailableAsync(
                    category,
                    bookingDate,
                    startTime,
                    endTime,
                    requiredCapacity,
                    requiredTools);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                throw new UserFriendlyException(
                    "An error occurred while checking room availability.",
                    details: e.Message);
            }
        }

        public async Task<BookingRequestDto> ProcessAsync(ProcessBookingRequestDto input)
        {
            // Validate booking request exists before proceeding
            if (!(await _repository.AnyAsync(br => br.Id == input.BookingRequestId)))
            {
                throw new UserFriendlyException(
                    "Booking request not found. It may have been deleted or already processed.",
                    details: $"Booking request ID: {input.BookingRequestId}");
            }

            var bookingRequest = await _repository.GetAsync(input.BookingRequestId);

            // If this is an approval with room assignment
            if (input.IsApproved && input.RoomId.HasValue)
            {
                // Update the RoomId on the booking request before approval
                bookingRequest.UpdateRoom(input.RoomId.Value);
                await _repository.UpdateAsync(bookingRequest);

                // Save changes before approval to ensure the update is persisted
                await CurrentUnitOfWork.SaveChangesAsync();

                // Approve the booking request
                await _roomBookingManager.ApproveBookingRequestAsync(
                    bookingRequest.Id,
                    _currentUser.Id.Value,
                    _currentUser.UserName
                );
            }
            else if (input.IsApproved)
            {
                // Standard approval without room change
                await _roomBookingManager.ApproveBookingRequestAsync(
                    input.BookingRequestId,
                    _currentUser.Id.Value,
                    _currentUser.UserName
                );
            }
            else
            {
                // Reject the booking request
                await _roomBookingManager.RejectBookingRequestAsync(
                    input.BookingRequestId,
                    _currentUser.Id.Value,
                    _currentUser.UserName,
                    input.RejectionReason
                );
            }

            // Reload the booking request
            bookingRequest = await _repository.GetAsync(input.BookingRequestId);

            return ObjectMapper.Map<BookingRequest, BookingRequestDto>(bookingRequest);
        }

        public async Task<List<BookingRequestDto>> GetMyRequestsAsync(DateTime? startDate = null, DateTime? endDate = null, Guid? roomId = null)
        {
            // Require authentication for viewing "my" requests
            if (!_currentUser.IsAuthenticated || !_currentUser.Id.HasValue)
            {
                return new List<BookingRequestDto>();
            }

            var query = await _repository.GetQueryableAsync();

            query = query
                .Include(br => br.Room);

            // Filter by date range if provided
            if (startDate.HasValue)
            {
                query = query.Where(br => br.BookingDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(br => br.BookingDate.Date <= endDate.Value.Date);
            }

            // Filter by room if provided
            if (roomId.HasValue)
            {
                query = query.Where(br => br.RoomId == roomId.Value);
            }

            // Order by booking date
            query = query.OrderBy(br => br.BookingDate);

            // Get the booking requests
            var bookingRequests = await query.ToListAsync();

            // Convert to DTOs
            var bookingRequestDtos = ObjectMapper.Map<List<BookingRequest>, List<BookingRequestDto>>(bookingRequests);

            return bookingRequestDtos;
        }

        [AllowAnonymous]
        public async Task<List<RoomDto>> GetAvailableRoomsAsync(
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            RoomCategory category,
            int requiredCapacity = 0,
            SoftwareTool requiredTools = SoftwareTool.None)
        {
            try
            {
                // Get all rooms that match the category
                var roomQuery = await _roomRepository.GetQueryableAsync();
                  roomQuery = roomQuery.Where(r => r.Category == category);

                // Filter by capacity if required
                if (requiredCapacity > 0)
                {
                    roomQuery = roomQuery.Where(r => r.Capacity >= requiredCapacity);
                }

                // Filter by required tools if specified
                if (requiredTools != SoftwareTool.None)
                {
                    roomQuery = roomQuery.Where(r => (r.AvailableTools & requiredTools) == requiredTools);
                }

                var rooms = await roomQuery.ToListAsync();

                // Check availability for each room
                var availableRooms = new List<Room>();
                foreach (var room in rooms)
                {
                    var isAvailable = await _roomBookingManager.IsRoomAvailableAsync(
                        room.Id,
                        bookingDate,
                        startTime,
                        endTime
                    );

                    if (isAvailable)
                    {
                        availableRooms.Add(room);
                    }
                }

                return ObjectMapper.Map<List<Room>, List<RoomDto>>(availableRooms);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                throw new UserFriendlyException(
                    "An error occurred while checking available rooms.",
                    details: e.Message);
            }
        }
    }
}
