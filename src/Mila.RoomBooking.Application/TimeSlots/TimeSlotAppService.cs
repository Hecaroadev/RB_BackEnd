// TimeSlotAppService.cs
using System;
using UniversityBooking.TimeSlots.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace UniversityBooking.TimeSlots
{
    public class TimeSlotAppService : 
        CrudAppService<
    TimeSlot,
    TimeSlotDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateTimeSlotDto,
    CreateUpdateTimeSlotDto>,
    ITimeSlotAppService
    {
    public TimeSlotAppService(IRepository<TimeSlot, Guid> repository)
        : base(repository)
    {
    }
    }
}