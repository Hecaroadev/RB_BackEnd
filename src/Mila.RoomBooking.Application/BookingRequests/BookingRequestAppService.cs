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
                .Include(br => br.Semester)
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

                if (input.RoomId == Guid.Empty)
                    throw new UserFriendlyException("Please select a valid room.");

                if (input.TimeSlotId == Guid.Empty)
                    throw new UserFriendlyException("Please select a valid time slot.");

                // Validate RequestedDate is not in the past
                if (input.RequestedDate.HasValue && input.RequestedDate.Value.Date < DateTime.Today)
                {
                    throw new UserFriendlyException("Booking date cannot be in the past.");
                }

                // Ensure user is authenticated
                if (_currentUser?.Id == null)
                {
                    throw new UserFriendlyException("You must be logged in to create a booking request.");
                }

                var currentUser = await _userRepository.GetAsync(_currentUser.Id.Value);

                // Check if the room is available before creating the request
                var isAvailable = await _roomBookingManager.IsRoomAvailableAsync(
                    input.RoomId,
                    input.TimeSlotId,
                    input.DayId,
                    input.SemesterId);

                if (!isAvailable)
                {
                    throw new UserFriendlyException(
                        "The room is not available for the selected time slot. " +
                        "Please choose a different time slot or room.");
                }

                var bookingRequest = await _roomBookingManager.CreateBookingRequestAsync(
                    input.RoomId,
                    input.TimeSlotId,
                    input.DayId,
                    input.SemesterId,
                    _currentUser.Id.Value,
                    _currentUser.UserName,
                    input.Purpose,
                    currentUser,
                    input.RequestedDate
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
                else if (e is RoomNotAvailableException)
                {
                    // Convert domain exception to user-friendly exception
                    throw new UserFriendlyException(
                        "The room is not available for the selected time slot. " +
                        "Please choose a different time slot or room.",
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

        [Authorize("UniversityBooking.BookingRequest.Manage")]
        public async Task<BookingRequestDto> ProcessAsync(ProcessBookingRequestDto input)
        {
            if (input.IsApproved)
            {
                // Approve the booking request
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

            var bookingRequest = await _repository.GetAsync(input.BookingRequestId);

            return ObjectMapper.Map<BookingRequest, BookingRequestDto>(bookingRequest);
        }

        public async Task<List<BookingRequestDto>> GetMyRequestsAsync(Guid? semesterId = null)
        {
            var query = await _repository.GetQueryableAsync();

            query = query
                .Where(br => br.RequestedById == _currentUser.Id)
                .Include(br => br.Room)
                .Include(br => br.TimeSlot)
                .Include(br => br.Day)
                .Include(br => br.Semester);

            if (semesterId.HasValue)
            {
                query = query.Where(br => br.SemesterId == semesterId.Value);
            }

            // Get the booking requests
            var bookingRequests = await query.ToListAsync();

            // Convert to DTOs
            var bookingRequestDtos = ObjectMapper.Map<List<BookingRequest>, List<BookingRequestDto>>(bookingRequests);

            return bookingRequestDtos;
        }
    }
}
