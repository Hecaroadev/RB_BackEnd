// DayDto.cs
using System;
using Volo.Abp.Application.Dtos;

namespace UniversityBooking.Days.Dtos
{
    public class DayDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}