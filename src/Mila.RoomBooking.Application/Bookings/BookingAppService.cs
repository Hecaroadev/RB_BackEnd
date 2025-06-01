// BookingAppService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UniversityBooking.Bookings.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace UniversityBooking.Bookings
{
    public class BookingAppService : ApplicationService, IBookingAppService
    {
        private readonly IRepository<Booking, Guid> _repository;
        private readonly ICurrentUser _currentUser;

        public BookingAppService(
            IRepository<Booking, Guid> repository,
            ICurrentUser currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<BookingDto> GetAsync(Guid id)
        {
            var booking = await _repository.GetAsync(id);

            /*
            // Check if the user is the creator of the booking or an admin
            if (booking.ReservedById != _currentUser.Id &&
                !await AuthorizationService.IsGrantedAsync("UniversityBooking.Booking.Manage"))
            {
                throw new UnauthorizedAccessException("You don't have permission to view this booking.");
            }
            */

            return ObjectMapper.Map<Booking, BookingDto>(booking);
        }

        public async Task<PagedResultDto<BookingDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
          // TODO permssions
            // Only admin users should access this method
            /*if (!await AuthorizationService.IsGrantedAsync("UniversityBooking.Booking.Manage"))
            {
                throw new UnauthorizedAccessException("You don't have permission to view all bookings.");
            }*/

            var query = await _repository.GetQueryableAsync();

            query = query
              .Include(b => b.Room);
            // Get total count
            var totalCount = await query.CountAsync();

            // Apply paging and sorting
            query = query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                // Implement sorting based on input.Sorting
                query = query.OrderByDescending(b => b.CreationTime);
            }
            else
            {
                // Default sorting
                query = query.OrderByDescending(b => b.CreationTime);
            }

            // Get the bookings
            var bookings = await query.ToListAsync();

            // Convert to DTOs
            var bookingDtos = ObjectMapper.Map<List<Booking>, List<BookingDto>>(bookings);

            return new PagedResultDto<BookingDto>(totalCount, bookingDtos);
        }

        public async Task<List<BookingDto>> GetByRoomAsync(Guid roomId)
        {
            var query = await _repository.GetQueryableAsync();

            query = query
              .Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active);

            // Get the bookings
            var bookings = await query.ToListAsync();

            // Convert to DTOs
            var bookingDtos = ObjectMapper.Map<List<Booking>, List<BookingDto>>(bookings);

            return bookingDtos;
        }

        public async Task<List<BookingDto>> GetByRoomAndDateAsync(Guid? roomId, DateTime? date)
        {
            var query = await _repository.GetQueryableAsync();

            // Apply room filter if provided
            if (roomId.HasValue && roomId.Value != Guid.Empty)
            {
                query = query.Where(b => b.RoomId == roomId.Value);
            }

            // Apply date filter if provided
            if (date.HasValue)
            {
                query = query.Where(b => b.BookingDate == null || b.BookingDate.Date == date.Value.Date);
            }

            // Only get active bookings
            query = query.Where(b => b.Status == BookingStatus.Active)
              .Include(b => b.Room);

            // Get the bookings
            var bookings = await query.ToListAsync();

            // Convert to DTOs
            var bookingDtos = ObjectMapper.Map<List<Booking>, List<BookingDto>>(bookings);

            return bookingDtos;
        }

        public async Task<List<BookingDto>> GetMyBookingsAsync()
        {
            var query = await _repository.GetQueryableAsync();

            query = query
              .Where(b => b.ReservedById == _currentUser.Id)
              .Include(b => b.Room);


            // Get the bookings
            var bookings = await query.ToListAsync();

            // Convert to DTOs
            var bookingDtos = ObjectMapper.Map<List<Booking>, List<BookingDto>>(bookings);

            return bookingDtos;
        }

        public async Task CancelAsync(Guid id, string reason)
        {
            var booking = await _repository.GetAsync(id);

            // Check if the user is the creator of the booking or an admin
            if (booking.ReservedById != _currentUser.Id &&
                !await AuthorizationService.IsGrantedAsync("UniversityBooking.Booking.Manage"))
            {
                throw new UnauthorizedAccessException("You don't have permission to cancel this booking.");
            }

            // Cancel the booking
            booking.Cancel(reason);

            await _repository.UpdateAsync(booking);
        }
    }
}
