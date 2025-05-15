// DayConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityBooking.Days;

namespace UniversityBooking.EntityFrameworkCore.Configurations
{
    public class DayConfiguration : IEntityTypeConfiguration<Day>
    {
        public void Configure(EntityTypeBuilder<Day> builder)
        {
            builder.ToTable("Days");

            builder.Property(x => x.Name).IsRequired().HasMaxLength(20);
            builder.Property(x => x.DayOfWeek).IsRequired();

            // Index for faster lookups
            builder.HasIndex(x => x.DayOfWeek).IsUnique();
        }
    }
}