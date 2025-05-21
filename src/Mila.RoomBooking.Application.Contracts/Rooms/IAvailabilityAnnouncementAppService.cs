using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityBooking.Rooms.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace UniversityBooking.Rooms
{
    public interface IAvailabilityAnnouncementAppService : IApplicationService
    {
        /// <summary>
        /// Get all active availability announcements
        /// </summary>
        Task<List<AvailabilityAnnouncementDto>> GetActiveAnnouncementsAsync();
        
        /// <summary>
        /// Get all announcements (including inactive ones)
        /// </summary>
        Task<PagedResultDto<AvailabilityAnnouncementDto>> GetAllAnnouncementsAsync(PagedAndSortedResultRequestDto input);
        
        /// <summary>
        /// Get a specific announcement by ID
        /// </summary>
        Task<AvailabilityAnnouncementDto> GetAnnouncementAsync(Guid id);
        
        /// <summary>
        /// Create a new availability announcement
        /// </summary>
        Task<AvailabilityAnnouncementDto> CreateAnnouncementAsync(CreateUpdateAvailabilityAnnouncementDto input);
        
        /// <summary>
        /// Update an existing announcement
        /// </summary>
        Task<AvailabilityAnnouncementDto> UpdateAnnouncementAsync(Guid id, CreateUpdateAvailabilityAnnouncementDto input);
        
        /// <summary>
        /// Delete an announcement
        /// </summary>
        Task DeleteAnnouncementAsync(Guid id);
        
        /// <summary>
        /// Get active announcements for a specific category
        /// </summary>
        Task<List<AvailabilityAnnouncementDto>> GetAnnouncementsByCategoryAsync(RoomCategory category);
    }
}