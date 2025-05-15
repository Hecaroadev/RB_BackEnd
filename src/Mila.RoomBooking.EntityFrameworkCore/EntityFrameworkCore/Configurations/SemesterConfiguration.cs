// SemesterConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityBooking.Semesters;

namespace UniversityBooking.EntityFrameworkCore.Configurations
{
    public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
    {
        public void Configure(EntityTypeBuilder<Semester> builder)
        {
            builder.ToTable("Semesters");

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.EndDate).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

            // Index for faster lookups
            builder.HasIndex(x => x.Name);
            builder.HasIndex(x => new { x.StartDate, x.EndDate });
        }
    }
}