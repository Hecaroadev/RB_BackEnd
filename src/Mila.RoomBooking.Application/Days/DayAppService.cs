// DayAppService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityBooking.Days.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace UniversityBooking.Days
{
    public class DayAppService : ApplicationService, IDayAppService
    {
        private readonly IRepository<Day, Guid> _repository;

        public DayAppService(IRepository<Day, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<List<DayDto>> GetListAsync()
        {
            var days = await _repository.GetListAsync();
            return ObjectMapper.Map<List<Day>, List<DayDto>>(days);
        }

        public async Task<DayDto> GetAsync(Guid id)
        {
            var day = await _repository.GetAsync(id);
            return ObjectMapper.Map<Day, DayDto>(day);
        }
    }
}