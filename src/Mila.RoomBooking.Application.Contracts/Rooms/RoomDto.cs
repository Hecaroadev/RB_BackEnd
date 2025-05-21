// RoomDto.cs
using System;
using System.Collections.Generic;
using System.Linq;
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
        public RoomCategory Category { get; set; }
        public string CategoryName => Category.ToString();
        public SoftwareTool AvailableTools { get; set; }
        public bool IsActive { get; set; }
        
        // Helper property to get the software tools as a list of strings
        public List<string> AvailableToolsList
        {
            get
            {
                if (AvailableTools == SoftwareTool.None)
                    return new List<string>();
                
                return Enum.GetValues(typeof(SoftwareTool))
                    .Cast<SoftwareTool>()
                    .Where(t => t != SoftwareTool.None && AvailableTools.HasFlag(t))
                    .Select(t => t.ToString())
                    .ToList();
            }
        }
    }
}