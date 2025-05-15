// IDayAppService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityBooking.Days.Dtos;
using Volo.Abp.Application.Services;

namespace UniversityBooking.Days
{
    public interface IDayAppService : IApplicationService
    {
        Task<List<DayDto>> GetListAsync();
        Task<DayDto> GetAsync(Guid id);
    }
}