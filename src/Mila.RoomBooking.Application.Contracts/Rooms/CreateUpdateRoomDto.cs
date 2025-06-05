// CreateUpdateRoomDto.cs
using System.ComponentModel.DataAnnotations;
using UniversityBooking.Rooms;

namespace UniversityBooking.Rooms.Dtos
{
    public class CreateUpdateRoomDto
    {
        [Required]
        [StringLength(50)]
        public string Number { get; set; }

        [Required]
        [StringLength(100)]
        public string Building { get; set; }

        [Required]
        [StringLength(50)]
        public string Floor { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Capacity { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public RoomType Type { get; set; }
        
        /// <summary>
        /// The category of room for booking process
        /// </summary>
        [Required]
        public RoomCategory Category { get; set; } = RoomCategory.Regular;
        
        /// <summary>
        /// Software tools available in this room (for labs)
        /// </summary>
        public SoftwareTool AvailableTools { get; set; } = SoftwareTool.None;

        public bool IsActive { get; set; } = true;
    }
}
