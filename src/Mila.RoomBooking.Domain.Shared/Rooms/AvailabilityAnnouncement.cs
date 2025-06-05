using System;

namespace UniversityBooking.Rooms
{
    /// <summary>
    /// DTO for an announcement of available dates and times for room bookings
    /// </summary>
    public class AvailabilityAnnouncementShared
    {
        /// <summary>
        /// Unique identifier for the announcement
        /// </summary>
        public Guid Id { get; set; }
        
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
        /// Optional minimum capacity for the rooms
        /// </summary>
        public int? MinCapacity { get; set; }
        
        /// <summary>
        /// Optional software tools available in these rooms
        /// </summary>
        public SoftwareTool AvailableTools { get; set; }
        
        /// <summary>
        /// When the announcement was created
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
        /// <summary>
        /// Who created the announcement
        /// </summary>
        public string CreatedBy { get; set; }
        
        /// <summary>
        /// Whether the announcement is currently active
        /// </summary>
        public bool IsActive { get; set; }
    }
}