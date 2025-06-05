using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace UniversityBooking.Rooms
{
    /// <summary>
    /// Represents an announcement of available dates and times for room bookings
    /// </summary>
    public class AvailabilityAnnouncement : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Title of the announcement
        /// </summary>
        public string Title { get; private set; }
        
        /// <summary>
        /// Description of the available times
        /// </summary>
        public string Description { get; private set; }
        
        /// <summary>
        /// Start date of the availability period
        /// </summary>
        public DateTime StartDate { get; private set; }
        
        /// <summary>
        /// End date of the availability period
        /// </summary>
        public DateTime EndDate { get; private set; }
        
        /// <summary>
        /// Category of rooms this announcement applies to
        /// </summary>
        public RoomCategory Category { get; private set; }
        
        /// <summary>
        /// Optional minimum capacity for the rooms
        /// </summary>
        public int? MinCapacity { get; private set; }
        
        /// <summary>
        /// Optional software tools available in these rooms
        /// </summary>
        public SoftwareTool AvailableTools { get; private set; }
        
        /// <summary>
        /// Whether the announcement is currently active
        /// </summary>
        public bool IsActive { get; private set; }
        
        protected AvailabilityAnnouncement()
        {
        }
        
        public AvailabilityAnnouncement(
            Guid id,
            string title,
            string description,
            DateTime startDate,
            DateTime endDate,
            RoomCategory category,
            int? minCapacity = null,
            SoftwareTool availableTools = SoftwareTool.None,
            bool isActive = true) : base(id)
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Category = category;
            MinCapacity = minCapacity;
            AvailableTools = availableTools;
            IsActive = isActive;
        }
        
        public void Update(
            string title,
            string description,
            DateTime startDate,
            DateTime endDate,
            RoomCategory category,
            int? minCapacity = null,
            SoftwareTool availableTools = SoftwareTool.None,
            bool isActive = true)
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Category = category;
            MinCapacity = minCapacity;
            AvailableTools = availableTools;
            IsActive = isActive;
        }
        
        public void Deactivate()
        {
            IsActive = false;
        }
        
        public void Activate()
        {
            IsActive = true;
        }
    }
}