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
using UniversityBooking.Days;
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
        private readonly IRepository<Day, Guid> _dayRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IIdentityUserRepository _userRepository;

        public BookingRequestAppService(
            IRepository<BookingRequest, Guid> repository,
            IRoomBookingManager roomBookingManager,
            IRepository<Booking, Guid> bookingRepository,
            IRepository<Day, Guid> dayRepository,
            ICurrentUser currentUser,
            IIdentityUserRepository userRepository)
        {
            _repository = repository;
            _roomBookingManager = roomBookingManager;
            _bookingRepository = bookingRepository;
            _dayRepository = dayRepository;
            _currentUser = currentUser;
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<BookingRequestDto>> GetPendingRequestsAsync(PagedAndSortedResultRequestDto input)
        {
            // Only admin users should access this method
            if (!await AuthorizationService.IsGrantedAsync("UniversityBooking.BookingRequest.Manage"))
            {
                throw new UnauthorizedAccessException("You don't have permission to view booking requests.");
            }

            var query = await _repository.GetQueryableAsync();

            query = query
                .Where(br => br.Status == BookingRequestStatus.Pending)
                .Include(br => br.Room)
                .Include(br => br.TimeSlot)
                .Include(br => br.Day)
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

                // Validate required fields for the new workflow
                if (input.Category == null)
                    throw new UserFriendlyException("Please select a room category.");

                if (input.DayId == null || input.DayId == Guid.Empty)
                    throw new UserFriendlyException("Please select a valid day.");

                if (string.IsNullOrWhiteSpace(input.StartTime) || string.IsNullOrWhiteSpace(input.EndTime))
                    throw new UserFriendlyException("Please specify start and end times.");

                if (string.IsNullOrWhiteSpace(input.Purpose))
                    throw new UserFriendlyException("Please specify the purpose of the booking.");

                if (string.IsNullOrWhiteSpace(input.InstructorName))
                    throw new UserFriendlyException("Please specify the instructor name.");

                if (string.IsNullOrWhiteSpace(input.Subject))
                    throw new UserFriendlyException("Please specify the subject.");

                // Validate booking date is not in the past
                if (input.BookingDate.HasValue && input.BookingDate.Value.Date < DateTime.Today)
                {
                    throw new UserFriendlyException("Booking date cannot be in the past.");
                }

                // Validate time range
                if (TimeSpan.TryParse(input.StartTime, out var startTime) &&
                    TimeSpan.TryParse(input.EndTime, out var endTime))
                {
                    if (endTime <= startTime)
                    {
                        throw new UserFriendlyException("End time must be after start time.");
                    }
                }
                else
                {
                    throw new UserFriendlyException("Invalid time format. Please use HH:mm format (e.g., 14:30).");
                }

                // Validate lab-specific requirements
                if (input.Category == RoomCategory.Lab)
                {
                    if (!input.NumberOfStudents.HasValue || input.NumberOfStudents.Value <= 0)
                    {
                        throw new UserFriendlyException("Number of students is required for lab bookings and must be greater than 0.");
                    }
                }

                // Validate recurring booking
                if (input.IsRecurring && (!input.RecurringWeeks.HasValue || input.RecurringWeeks.Value <= 0 || input.RecurringWeeks.Value > 16))
                {
                    throw new UserFriendlyException("Recurring weeks must be between 1 and 16.");
                }

                // Ensure user is authenticated
                if (_currentUser?.Id == null)
                {
                    throw new UserFriendlyException("You must be logged in to create a booking request.");
                }

                var currentUser = await _userRepository.GetAsync(_currentUser.Id.Value);

                // Validate that the day exists
                var day = await _dayRepository.GetAsync(input.DayId.Value);
                if (day == null)
                {
                    throw new UserFriendlyException("Invalid day selected.");
                }

                // Create booking request using the new workflow
                var bookingRequest = await _roomBookingManager.CreateTimeRangeBookingRequestAsync(
                    input.Category.Value,
                    input.DayId.Value,
                    input.BookingDate ?? DateTime.Today,
                    input.StartTime,
                    input.EndTime,
                    _currentUser.Id.Value,
                    _currentUser.UserName,
                    input.Purpose,
                    input.InstructorName,
                    input.Subject,
                    input.NumberOfStudents ?? 0,
                    input.RequiredTools ?? SoftwareTool.None,
                    input.IsRecurring,
                    input.RecurringWeeks ?? 0,
                    currentUser
                );

                return ObjectMapper.Map<BookingRequest, BookingRequestDto>(bookingRequest);
            }
            catch (Exception e)
            {
                // Add more context to the error message if it's not already a UserFriendlyException
                if (e is UserFriendlyException)
                {
                    // Just rethrow user-friendly exceptions
                    throw;
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

        [Authorize("UniversityBooking.BookingRequest.Manage")]
        public async Task<BookingRequestDto> ProcessAsync(ProcessBookingRequestDto input)
        {
            try
            {
                // Validate that the booking request exists
                var bookingRequestExists = await _repository.AnyAsync(br => br.Id == input.BookingRequestId);
                if (!bookingRequestExists)
                {
                    throw new UserFriendlyException("Booking request not found. It may have been deleted or processed by another administrator.");
                }

                if (input.IsApproved)
                {
                    // For approval, we need room assignment
                    if (!input.RoomId.HasValue)
                    {
                        throw new UserFriendlyException("Please select a room to assign for this booking.");
                    }

                    // Approve the booking request with room assignment
                    await _roomBookingManager.ApproveBookingRequestWithRoomAssignmentAsync(
                        input.BookingRequestId,
                        input.RoomId,
                        _currentUser.Id.Value,
                        _currentUser.UserName
                    );
                }
                else
                {
                    // Reject the booking request
                    if (string.IsNullOrWhiteSpace(input.RejectionReason))
                    {
                        throw new UserFriendlyException("Please provide a reason for rejection.");
                    }

                    await _roomBookingManager.RejectBookingRequestAsync(
                        input.BookingRequestId,
                        _currentUser.Id.Value,
                        _currentUser.UserName,
                        input.RejectionReason
                    );
                }

                var bookingRequest = await _repository.GetAsync(input.BookingRequestId);
                return ObjectMapper.Map<BookingRequest, BookingRequestDto>(bookingRequest);
            }
            catch (Exception e)
            {
                if (e is UserFriendlyException)
                {
                    throw;
                }

                Logger.LogException(e);
                throw new UserFriendlyException(
                    "An error occurred while processing the booking request. Please try again.",
                    details: e.Message);
            }
        }

        public async Task<List<BookingRequestDto>> GetMyRequestsAsync()
        {
            var query = await _repository.GetQueryableAsync();

            query = query
                .Where(br => br.RequestedById == _currentUser.Id)
                .Include(br => br.Room)
                .Include(br => br.TimeSlot)
                .Include(br => br.Day)
                .OrderByDescending(br => br.CreationTime);

            // Get the booking requests
            var bookingRequests = await query.ToListAsync();

            // Convert to DTOs
            var bookingRequestDtos = ObjectMapper.Map<List<BookingRequest>, List<BookingRequestDto>>(bookingRequests);

            return bookingRequestDtos;
        }

        public Task<bool> IsCategoryAvailableAsync(RoomCategory category, Guid dayId, DateTime bookingDate, TimeSpan startTime,
          TimeSpan endTime, int requiredCapacity = 0, SoftwareTool requiredTools = SoftwareTool.None)
        {
          throw new NotImplementedException();
        }
    }
}
