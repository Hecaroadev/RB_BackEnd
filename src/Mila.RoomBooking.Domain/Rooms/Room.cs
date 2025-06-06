// Room.cs
using System;
using System.Collections.Generic;
using UniversityBooking.BookingRequests;
using UniversityBooking.Bookings;
using Volo.Abp.Domain.Entities.Auditing;

namespace UniversityBooking.Rooms
{
    public class Room : FullAuditedAggregateRoot<Guid>
    {
        public string Number { get; private set; }
        public string Building { get; private set; }
        public string Floor { get; private set; }
        public int Capacity { get; private set; }
        public string Description { get; private set; }
        public RoomType Type { get; private set; }
        public bool IsActive { get; set; }
        
        /// <summary>
        /// The category of room for booking process
        /// </summary>
        public RoomCategory Category { get; private set; }
        
        /// <summary>
        /// Software tools available in this room (if applicable)
        /// </summary>
        public SoftwareTool AvailableTools { get; private set; }
        
        public virtual ICollection<Booking> Bookings { get; private set; }
        public virtual ICollection<BookingRequest> BookingRequests { get; private set; }

        protected Room()
        {
        }

        public Room(
            Guid id,
            string number,
            string building,
            string floor,
            int capacity,
            RoomType type,
            string description = null,
            RoomCategory category = RoomCategory.Regular,
            SoftwareTool availableTools = SoftwareTool.None
        ) : base(id)
        {
            Number = number;
            Building = building;
            Floor = floor;
            Capacity = capacity;
            Type = type;
            Description = description;
            Category = category;
            AvailableTools = availableTools;
            IsActive = true;
            
            Bookings = new List<Booking>();
            BookingRequests = new List<BookingRequest>();
        }

        public void Update(
            string number,
            string building,
            string floor,
            int capacity,
            RoomType type,
            string description = null,
            RoomCategory category = RoomCategory.Regular,
            SoftwareTool availableTools = SoftwareTool.None)
        {
            Number = number;
            Building = building;
            Floor = floor;
            Capacity = capacity;
            Type = type;
            Description = description;
            Category = category;
            AvailableTools = availableTools;
        }
    }

    public enum RoomType
    {
        Classroom,
        Laboratory,
        LectureHall,
        MeetingRoom,
        ComputerLab
    }
}