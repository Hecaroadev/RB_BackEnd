using System;

namespace UniversityBooking.Rooms
{
    /// <summary>
    /// Room category for different types of booking processes
    /// </summary>
    public enum RoomCategory
    {
        /// <summary>
        /// Regular room booking
        /// </summary>
        Regular = 0,
        
        /// <summary>
        /// Laboratory rooms with capacity and software requirements
        /// Room name is hidden from users during booking
        /// </summary>
        Lab = 1,
        
        /// <summary>
        /// Rooms for stream classes
        /// </summary>
        StreamClass = 2,
        
        /// <summary>
        /// Rooms for press conferences
        /// </summary>
        PressConference = 3
    }
}