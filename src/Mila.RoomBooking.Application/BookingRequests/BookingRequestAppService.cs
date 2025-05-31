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

        public BookingRequestAppService(
            IRepository<BookingRequest, Guid> repository,
            IRoomBookingManager roomBookingManager,
            IRepository<Booking, Guid> bookingRepository,
            ICurrentUser currentUser,
            IIdentityUserRepository userRepository)
        {
            _repository = repository;
            _roomBookingManager = roomBookingManager;
            _bookingRepository = bookingRepository;
            _currentUser = currentUser;
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<BookingRequestDto>> GetPendingRequestsAsync(PagedAndSortedResultRequestDto input)
        {
          // TODO permissions
            /*
            // Only admin users should access this method
            if (!await AuthorizationService.IsGrantedAsync("UniversityBooking.BookingRequest.Manage"))
            {
                throw new UnauthorizedAccessException("You don't have permission to view booking requests.");
            }
            */

            var query = await _repository.GetQueryableAsync();

            query = query
                .Where(br => br.Status == BookingRequestStatus.Pending)
                .Include(br => br.Room)
                .Include(br => br.RequestedByUser);

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply paging and sorting
            query = query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                // Implement sorting based on input.Sorting
                query = query.OrderBy(br => br.RequestDate);
            }
            else
            {
                // Default sorting
                query = query.OrderBy(br => br.RequestDate);
            }

            // Get the booking requests
            var bookingRequests = await query.ToListAsync();

            // Convert to DTOs
            var bookingRequestDtos = ObjectMapper.Map<List<BookingRequest>, List<BookingRequestDto>>(bookingRequests);

            return new PagedResultDto<BookingRequestDto>(totalCount, bookingRequestDtos);
        }

        public async Task<PagedResultDto<BookingRequestDto>> GetApprovedRequestsAsync(PagedAndSortedResultRequestDto input)
        {
          // TODO permissions
            /*
            // Only admin users should access this method
            if (!await AuthorizationService.IsGrantedAsync("UniversityBooking.BookingRequest.Manage"))
            {
                throw new UnauthorizedAccessException("You don't have permission to view booking requests.");
            }
            */

            var query = await _repository.GetQueryableAsync();

            query = query
                .Where(br => br.Status == BookingRequestStatus.Approved)
                .Include(br => br.Room)
                .Include(br => br.RequestedByUser);

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply paging and sorting
            query = query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                // Implement sorting based on input.Sorting
                query = query.OrderBy(br => br.RequestDate);
            }
            else
            {
                // Default sorting
                query = query.OrderBy(br => br.RequestDate);
            }

            // Get the booking requests
            var bookingRequests = await query.ToListAsync();

            // Convert to DTOs
            var bookingRequestDtos = ObjectMapper.Map<List<BookingRequest>, List<BookingRequestDto>>(bookingRequests);

            return new PagedResultDto<BookingRequestDto>(totalCount, bookingRequestDtos);
        }

        public async Task<BookingRequestDto> GetAsync(Guid id)
        {
            var bookingRequest = await _repository.GetAsync(id);

            // Check if the user is the creator of the booking request or an admin
            if (bookingRequest.RequestedById != _currentUser.Id &&
                !await AuthorizationService.IsGrantedAsync("UniversityBooking.BookingRequest.Manage"))
            {
                throw new UnauthorizedAccessException("You don't have permission to view this booking request.");
            }

            return ObjectMapper.Map<BookingRequest, BookingRequestDto>(bookingRequest);
        }

        public async Task<BookingRequestDto> CreateAsync(CreateBookingRequestDto input)
        {
            try
            {
                // Validate input
                if (input == null)
                    throw new UserFriendlyException("Invalid booking request data.");

                // Room selection is now optional for all categories - will be assigned by admin

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

                // Ensure user is authenticated
                if (_currentUser?.Id == null)
                {
                    throw new UserFriendlyException("You must be logged in to create a booking request.");
                }

                var currentUser = await _userRepository.GetAsync(_currentUser.Id.Value);

                // Handle booking creation based on category
                BookingRequest bookingRequest;

                // Use the enhanced booking creation for all requests
                bookingRequest = await _roomBookingManager.CreateEnhancedBookingRequestAsync(
                    input.RoomId,
                    _currentUser.Id.Value,
                    _currentUser.UserName,
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
                    DateTime.Now // Default to current time for RequestedDate
                );

                return ObjectMapper.Map<BookingRequest, BookingRequestDto>(bookingRequest);
            }            catch (Exception e)
            {
                // Add more context to the error message if it's not already a UserFriendlyException
                if (e is UserFriendlyException)
                {
                    // Just rethrow user-friendly exceptions
                    throw;
                }
                else if (e is RoomNotAvailableException)
                {
                    // Convert domain exception to user-friendly exception
                    throw new UserFriendlyException(
                        "No room is available that meets your requirements. " +
                        "Please try a different time or adjust your requirements.",
                        details: e.Message);
                }
                else if (e is ArgumentNullException)
                {
                    // Handle null argument exceptions
                    throw new UserFriendlyException(
                        "Missing required information. " +
                        "Please fill in all required fields.",
                        details: e.Message);
                }
                else
                {
                    // Log the raw exception
                    Logger.LogException(e);

                    // Return a sanitized exception to the user
                    throw new UserFriendlyException(
                        "An error occurred while processing your booking request. " +
                        "Please try again or contact support if the problem persists.",
                        details: $"Error ID: {Guid.NewGuid()}");
                }
            }
        }

        /// <summary>
        /// Check if a room category is available for the specified time range
        /// </summary>
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
            var bookingRequest = await _repository.GetAsync(input.BookingRequestId);

            // If this is an approval with room assignment
            if (input.IsApproved && input.RoomId.HasValue)
            {
                // Update the RoomId on the booking request before approval
                bookingRequest.UpdateRoom(input.RoomId.Value);
                await _repository.UpdateAsync(bookingRequest);

                // Approve the booking request
                await _roomBookingManager.ApproveBookingRequestAsync(
                    input.BookingRequestId,
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

        /*
        public async Task<List<BookingRequestDto>> GetMyRequestsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = await _repository.GetQueryableAsync();

            query = query
              .Where(br => br.RequestedById == _currentUser.Id)
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

            // Get the booking requests
            var bookingRequests = await query.ToListAsync();

            // Convert to DTOs
            var bookingRequestDtos = ObjectMapper.Map<List<BookingRequest>, List<BookingRequestDto>>(bookingRequests);

            return bookingRequestDtos;
        }
        */
        // Updated GetMyRequestsAsync method in BookingRequestAppService.cs
        public async Task<List<BookingRequestDto>> GetMyRequestsAsync(DateTime? startDate = null, DateTime? endDate = null, Guid? roomId = null)
        {
          var query = await _repository.GetQueryableAsync();

          query = query
            .Where(br => br.Status != BookingRequestStatus.Pending)
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

    }
}
