// Semester.cs
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace UniversityBooking.Semesters
{
    public class Semester : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public bool IsActive { get; set; }
        
        protected Semester()
        {
        }

        public Semester(
            Guid id,
            string name,
            DateTime startDate,
            DateTime endDate
        ) : base(id)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = true;
        }

        public void Update(string name, DateTime startDate, DateTime endDate)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public bool IsDateInSemester(DateTime date)
        {
            return date.Date >= StartDate.Date && date.Date <= EndDate.Date;
        }
    }
}