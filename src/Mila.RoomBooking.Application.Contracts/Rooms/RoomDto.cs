// RoomDto.cs
using System;
using UniversityBooking.Rooms;
using Volo.Abp.Application.Dtos;

namespace UniversityBooking.Rooms.Dtos
{
    public class RoomDto : EntityDto<Guid>
    {
        public string Number { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public RoomType Type { get; set; }
        public string TypeName => Type.ToString();
        public bool IsActive { get; set; }
    }
}