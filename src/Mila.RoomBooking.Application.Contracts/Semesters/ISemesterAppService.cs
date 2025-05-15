// ISemesterAppService.cs
using System;
using System.Threading.Tasks;
using UniversityBooking.Semesters.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace UniversityBooking.Semesters
{
    public interface ISemesterAppService : 
        ICrudAppService<
    SemesterDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateSemesterDto,
    CreateUpdateSemesterDto>
    {
    Task<SemesterDto> GetCurrentSemesterAsync();
    }
}