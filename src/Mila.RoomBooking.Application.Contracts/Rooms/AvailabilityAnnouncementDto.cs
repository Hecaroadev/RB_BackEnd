using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace UniversityBooking.Rooms.Dtos
{
    /// <summary>
    /// Data transfer object for room availability announcements
    /// </summary>
    public class AvailabilityAnnouncementDto : EntityDto<Guid>
    {
        /// <summary>
        /// Title of the announcement
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Description of the available times
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Start date of the availability period
        /// </summary>
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// End date of the availability period
        /// </summary>
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Category of rooms this announcement applies to
        /// </summary>
        public RoomCategory Category { get; set; }
        
        /// <summary>
        /// String representation of the category
        /// </summary>
        public string CategoryName => Category.ToString();
        
        /// <summary>
        /// Optional minimum capacity for the rooms
        /// </summary>
        public int? MinCapacity { get; set; }
        
        /// <summary>
        /// Optional software tools available in these rooms
        /// </summary>
        public SoftwareTool AvailableTools { get; set; }
        
        /// <summary>
        /// Who created the announcement
        /// </summary>
        public string CreatedBy { get; set; }
        
        /// <summary>
        /// When the announcement was created
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
        /// <summary>
        /// Whether the announcement is currently active
        /// </summary>
        public bool IsActive { get; set; }
        
        // Formatted dates for display
        public string FormattedStartDate => StartDate.ToString("yyyy-MM-dd");
        public string FormattedEndDate => EndDate.ToString("yyyy-MM-dd");
        public string DateRange => $"{FormattedStartDate} - {FormattedEndDate}";
    }
    
    /// <summary>
    /// DTO for creating or updating availability announcements
    /// </summary>
    public class CreateUpdateAvailabilityAnnouncementDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string Description { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        public RoomCategory Category { get; set; }
        
        public int? MinCapacity { get; set; }
        
        public SoftwareTool AvailableTools { get; set; } = SoftwareTool.None;
        
        public bool IsActive { get; set; } = true;
    }
}