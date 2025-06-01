// CalendarDayConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityBooking.Calendar;

namespace UniversityBooking.EntityFrameworkCore.Configurations
{
  public class CalendarDayConfiguration : IEntityTypeConfiguration<CalendarDay>
  {
    public void Configure(EntityTypeBuilder<CalendarDay> builder)
    {
      builder.ToTable("CalendarDays");

      // Since CalendarDay uses Date as its key (from GetKeys method)
      builder.HasKey(x => x.Date);

      builder.Property(x => x.Date).IsRequired();
      builder.Property(x => x.DayOfWeek).IsRequired();
      builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(50);
      builder.Property(x => x.IsWeekend).IsRequired();
      builder.Property(x => x.IsToday).IsRequired();
      builder.Property(x => x.IsInSemester).IsRequired();
      builder.Property(x => x.DayId).IsRequired(false);

      // Index for faster lookups
      builder.HasIndex(x => x.DayOfWeek);
      builder.HasIndex(x => x.IsInSemester);
      builder.HasIndex(x => x.DayId);
      builder.HasIndex(x => new { x.Date, x.IsInSemester });

      // Relationship with Day
      builder
        .HasOne(cd => cd.Day)
        .WithMany()
        .HasForeignKey(cd => cd.DayId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}
