// CreateUpdateSemesterDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace UniversityBooking.Semesters.Dtos
{
    public class CreateUpdateSemesterDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; }
    }
}