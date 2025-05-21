// RoomAppService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal.Mappers;
using Microsoft.EntityFrameworkCore;
using UniversityBooking.BookingRequests;
using UniversityBooking.Bookings;
using UniversityBooking.Bookings.Dtos;
using UniversityBooking.Rooms.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace UniversityBooking.Rooms
{
    public class RoomAppService : 
        CrudAppService<
            Room,
            RoomDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateRoomDto,
            CreateUpdateRoomDto>,
        IRoomAppService
    {
        private readonly IRepository<Booking, Guid> _bookingRepository;
        private readonly IRepository<BookingRequest, Guid> _bookingRequestRepository;
        
        public RoomAppService(
            IRepository<Room, Guid> repository,
            IRepository<Booking, Guid> bookingRepository,
            IRepository<BookingRequest, Guid> bookingRequestRepository)
            : base(repository)
        {
            _bookingRepository = bookingRepository;
            _bookingRequestRepository = bookingRequestRepository;
        }

        public async Task<List<BookingDto>> GetBookingsAsync(Guid id, Guid? semesterId = null)
        {
            var query = await _bookingRepository.GetQueryableAsync();
            
            query = query
                .Where(b => b.RoomId == id && b.Status == BookingStatus.Active)
                .Include(b => b.TimeSlot)
                .Include(b => b.Day)
                .Include(b => b.Semester);
                
            if (semesterId.HasValue)
            {
                query = query.Where(b => b.SemesterId == semesterId.Value);
            }
            
            var bookings = await query.ToListAsync();
            
            return ObjectMapper.Map<List<Booking>, List<BookingDto>>(bookings);
        }

        public async Task<PagedResultDto<RoomDto>> GetAvailableRoomsAsync(GetAvailableRoomsInput input)
        {
            // Get bookings for the specified time slot, day, and semester
            var bookingQuery = await _bookingRepository.GetQueryableAsync();
            
            var bookedRoomIds = bookingQuery
                .Where(b => 
                    b.TimeSlotId == input.TimeSlotId && 
                    b.DayId == input.DayId && 
                    b.Status == BookingStatus.Active)
                .Select(b => b.RoomId);
            
            // Get pending booking requests for the specified time slot, day, and semester
            var requestQuery = await _bookingRequestRepository.GetQueryableAsync();
            
            var pendingRequestRoomIds = requestQuery
                .Where(br => 
                    br.TimeSlotId == input.TimeSlotId && 
                    br.DayId == input.DayId && 
                    br.Status == BookingRequests.BookingRequestStatus.Pending)
                .Select(br => br.RoomId);
            
            // Get all available rooms
            var query = await Repository.GetQueryableAsync();
            
            query = query
                .Where(r => r.IsActive)
                .Where(r => !bookedRoomIds.Contains(r.Id))
                .Where(r => !pendingRequestRoomIds.Contains(r.Id));
            
            // Apply filter if provided
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(r => 
                    r.Number.Contains(input.Filter) || 
                    r.Building.Contains(input.Filter) || 
                    r.Description.Contains(input.Filter));
            }
            
            // Get total count
            var totalCount = await query.CountAsync();
            
            // Apply paging and sorting
            query = query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);
                
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                // Implement sorting based on input.Sorting
                query = query.OrderBy(r => r.Building).ThenBy(r => r.Number);
            }
            else
            {
                // Default sorting
                query = query.OrderBy(r => r.Building).ThenBy(r => r.Number);
            }
            
            // Get the rooms
            var rooms = await query.ToListAsync();
            
            // Convert to DTOs
            var roomDtos = ObjectMapper.Map<List<Room>, List<RoomDto>>(rooms);
            
            return new PagedResultDto<RoomDto>(totalCount, roomDtos);
        }
    }
}