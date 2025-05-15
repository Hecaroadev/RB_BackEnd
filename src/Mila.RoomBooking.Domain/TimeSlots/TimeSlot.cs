// TimeSlot.cs
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace UniversityBooking.TimeSlots
{
    public class TimeSlot : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public bool IsActive { get; set; }

        protected TimeSlot()
        {
        }

        public TimeSlot(
            Guid id,
            string name,
            TimeSpan startTime,
            TimeSpan endTime
        ) : base(id)
        {
            Name = name;
            StartTime = startTime;
            EndTime = endTime;
            IsActive = true;
        }

        public void Update(string name, TimeSpan startTime, TimeSpan endTime)
        {
            Name = name;
            StartTime = startTime;
            EndTime = endTime;
        }

        public bool Overlaps(TimeSlot other)
        {
            return (StartTime < other.EndTime && EndTime > other.StartTime);
        }
    }
}