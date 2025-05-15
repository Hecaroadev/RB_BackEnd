// TimeSlotConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityBooking.TimeSlots;

namespace UniversityBooking.EntityFrameworkCore.Configurations
{
    public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
    {
        public void Configure(EntityTypeBuilder<TimeSlot> builder)
        {
            builder.ToTable("TimeSlots");

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.StartTime).IsRequired();
            builder.Property(x => x.EndTime).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

            // Index for faster lookups
            builder.HasIndex(x => x.Name);
            builder.HasIndex(x => new { x.StartTime, x.EndTime });
        }
    }
}