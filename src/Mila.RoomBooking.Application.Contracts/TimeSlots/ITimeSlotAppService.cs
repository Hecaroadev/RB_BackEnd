// ITimeSlotAppService.cs
using System;
using UniversityBooking.TimeSlots.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace UniversityBooking.TimeSlots
{
    public interface ITimeSlotAppService : 
        ICrudAppService<
    TimeSlotDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateTimeSlotDto,
    CreateUpdateTimeSlotDto>
    {
    }
}