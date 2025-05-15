// SemesterAppService.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal.Mappers;
using Microsoft.EntityFrameworkCore;
using UniversityBooking.Semesters.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace UniversityBooking.Semesters
{
    public class SemesterAppService : 
        CrudAppService<
            Semester,
            SemesterDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateSemesterDto,
            CreateUpdateSemesterDto>,
        ISemesterAppService
    {
        public SemesterAppService(IRepository<Semester, Guid> repository)
            : base(repository)
        {
        }

        public async Task<SemesterDto> GetCurrentSemesterAsync()
        {
            var query = await Repository.GetQueryableAsync();
            
            // Try to get the active semester first
            var activeSemester = await query
                .Where(s => s.IsActive)
                .FirstOrDefaultAsync();
                
            if (activeSemester != null)
            {
                return ObjectMapper.Map<Semester, SemesterDto>(activeSemester);
            }
            
            // If no active semester, get the semester that contains the current date
            var today = DateTime.Today;
            var currentSemester = await query
                .Where(s => s.StartDate <= today && s.EndDate >= today)
                .FirstOrDefaultAsync();
                
            if (currentSemester != null)
            {
                return ObjectMapper.Map<Semester, SemesterDto>(currentSemester);
            }
            
            // If no current semester, get the nearest future semester
            var futureSemester = await query
                .Where(s => s.StartDate > today)
                .OrderBy(s => s.StartDate)
                .FirstOrDefaultAsync();
                
            if (futureSemester != null)
            {
                return ObjectMapper.Map<Semester, SemesterDto>(futureSemester);
            }
            
            // If no future semester, get the most recent past semester
            var pastSemester = await query
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();
                
            if (pastSemester != null)
            {
                return ObjectMapper.Map<Semester, SemesterDto>(pastSemester);
            }
            
            // If no semesters at all, return null
            return null;
        }
    }
}