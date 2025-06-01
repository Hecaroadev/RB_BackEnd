// CalendarWeekConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityBooking.Calendar;

namespace UniversityBooking.EntityFrameworkCore.Configurations
{
    public class CalendarWeekConfiguration : IEntityTypeConfiguration<CalendarWeek>
    {
        public void Configure(EntityTypeBuilder<CalendarWeek> builder)
        {
            builder.ToTable("CalendarWeeks");

            // Since CalendarWeek uses WeekStartDate as its key (from GetKeys method)
            builder.HasKey(x => x.WeekStartDate);

            builder.Property(x => x.WeekStartDate).IsRequired();
            builder.Property(x => x.WeekEndDate).IsRequired();
            builder.Property(x => x.WeekNumber).IsRequired();
            builder.Property(x => x.Year).IsRequired();

            // Configure the Days collection as Owned entities since they are part of the aggregate
            builder.OwnsMany(x => x.Days, d =>
            {
                d.ToTable("CalendarWeekDays");
                d.WithOwner().HasForeignKey("CalendarWeekWeekStartDate");
                d.Property(x => x.Date).IsRequired();
                d.Property(x => x.DayOfWeek).IsRequired();
                d.Property(x => x.DisplayName).IsRequired().HasMaxLength(50);
                d.Property(x => x.IsWeekend).IsRequired();
                d.Property(x => x.IsToday).IsRequired();
                d.Property(x => x.IsInSemester).IsRequired();
                d.Property(x => x.DayId).IsRequired(false);
                d.HasKey("CalendarWeekWeekStartDate", "Date");

                // Configure relationship with Day entity within owned entity
                d.HasOne(cd => cd.Day)
                    .WithMany()
                    .HasForeignKey(cd => cd.DayId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Index for faster lookups
            builder.HasIndex(x => x.WeekNumber);
            builder.HasIndex(x => x.Year);
            builder.HasIndex(x => new { x.Year, x.WeekNumber });
            builder.HasIndex(x => new { x.WeekStartDate, x.WeekEndDate });
        }
    }
}
