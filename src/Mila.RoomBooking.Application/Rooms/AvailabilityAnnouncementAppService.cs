using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityBooking.Rooms.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

// Use Domain version explicitly
using AvailabilityAnnouncementEntity = UniversityBooking.Rooms.AvailabilityAnnouncement;

namespace UniversityBooking.Rooms
{
    // Only administrators can create/edit/delete announcements
    [Authorize]
    public class AvailabilityAnnouncementAppService : ApplicationService, IAvailabilityAnnouncementAppService
    {
        private readonly IRepository<AvailabilityAnnouncementEntity, Guid> _repository;
        
        public AvailabilityAnnouncementAppService(
            IRepository<AvailabilityAnnouncementEntity, Guid> repository)
        {
            _repository = repository;
        }
        
        // Any user can view active announcements
        [AllowAnonymous]
        public async Task<List<AvailabilityAnnouncementDto>> GetActiveAnnouncementsAsync()
        {
            var query = await _repository.GetQueryableAsync();
            
            var announcements = await query
                .Where(a => a.IsActive && a.EndDate >= DateTime.Now)
                .OrderByDescending(a => a.CreationTime)
                .ToListAsync();
                
            return ObjectMapper.Map<List<AvailabilityAnnouncementEntity>, List<AvailabilityAnnouncementDto>>(announcements);
        }
        
        // Admins can view all announcements
        [Authorize("UniversityBooking.Admin")]
        public async Task<PagedResultDto<AvailabilityAnnouncementDto>> GetAllAnnouncementsAsync(
            PagedAndSortedResultRequestDto input)
        {
            var query = await _repository.GetQueryableAsync();
            
            var totalCount = await query.CountAsync();
            
            query = query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .OrderByDescending(a => a.CreationTime);
                
            var announcements = await query.ToListAsync();
            
            return new PagedResultDto<AvailabilityAnnouncementDto>(
                totalCount,
                ObjectMapper.Map<List<AvailabilityAnnouncementEntity>, List<AvailabilityAnnouncementDto>>(announcements)
            );
        }
        
        // Any user can view a specific announcement
        [AllowAnonymous]
        public async Task<AvailabilityAnnouncementDto> GetAnnouncementAsync(Guid id)
        {
            var announcement = await _repository.GetAsync(id);
            return ObjectMapper.Map<AvailabilityAnnouncementEntity, AvailabilityAnnouncementDto>(announcement);
        }
        
        // Only admins can create announcements
        [Authorize("UniversityBooking.Admin")]
        public async Task<AvailabilityAnnouncementDto> CreateAnnouncementAsync(
            CreateUpdateAvailabilityAnnouncementDto input)
        {
            // Validate date range
            if (input.EndDate < input.StartDate)
            {
                throw new UserFriendlyException("End date must be after start date.");
            }
            
            var announcement = new AvailabilityAnnouncementEntity(
                GuidGenerator.Create(),
                input.Title,
                input.Description,
                input.StartDate,
                input.EndDate,
                input.Category,
                input.MinCapacity,
                input.AvailableTools,
                input.IsActive
            );
            
            await _repository.InsertAsync(announcement);
            
            return ObjectMapper.Map<AvailabilityAnnouncementEntity, AvailabilityAnnouncementDto>(announcement);
        }
        
        // Only admins can update announcements
        [Authorize("UniversityBooking.Admin")]
        public async Task<AvailabilityAnnouncementDto> UpdateAnnouncementAsync(
            Guid id, 
            CreateUpdateAvailabilityAnnouncementDto input)
        {
            // Validate date range
            if (input.EndDate < input.StartDate)
            {
                throw new UserFriendlyException("End date must be after start date.");
            }
            
            var announcement = await _repository.GetAsync(id);
            
            announcement.Update(
                input.Title,
                input.Description,
                input.StartDate,
                input.EndDate,
                input.Category,
                input.MinCapacity,
                input.AvailableTools,
                input.IsActive
            );
            
            await _repository.UpdateAsync(announcement);
            
            return ObjectMapper.Map<AvailabilityAnnouncementEntity, AvailabilityAnnouncementDto>(announcement);
        }
        
        // Only admins can delete announcements
        [Authorize("UniversityBooking.Admin")]
        public async Task DeleteAnnouncementAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
        
        // Any user can view active announcements by category
        [AllowAnonymous]
        public async Task<List<AvailabilityAnnouncementDto>> GetAnnouncementsByCategoryAsync(RoomCategory category)
        {
            var query = await _repository.GetQueryableAsync();
            
            var announcements = await query
                .Where(a => a.IsActive && a.EndDate >= DateTime.Now && a.Category == category)
                .OrderByDescending(a => a.CreationTime)
                .ToListAsync();
                
            return ObjectMapper.Map<List<AvailabilityAnnouncementEntity>, List<AvailabilityAnnouncementDto>>(announcements);
        }
    }
}