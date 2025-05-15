// SemesterDto.cs
using System;
using Volo.Abp.Application.Dtos;

namespace UniversityBooking.Semesters.Dtos
{
    public class SemesterDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        
        // Formatted dates for display
        public string FormattedStartDate => StartDate.ToShortDateString();
        public string FormattedEndDate => EndDate.ToShortDateString();
        public string DisplayName => $"{Name} ({FormattedStartDate} - {FormattedEndDate})";
    }
}