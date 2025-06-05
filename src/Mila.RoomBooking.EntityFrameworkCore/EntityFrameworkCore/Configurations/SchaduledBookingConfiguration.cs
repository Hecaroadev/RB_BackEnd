using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mila.RoomBooking.SchaduledBookings;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Mila.RoomBooking.EntityFrameworkCore.Configurations
{
    public class SchaduledBookingConfiguration : IEntityTypeConfiguration<SchaduledBooking>
    {
        public void Configure(EntityTypeBuilder<SchaduledBooking> builder)
        {
            builder.ToTable("SchaduledBookings");

            builder.ConfigureByConvention();

            // Configure properties
            builder.Property(x => x.Purpose)
                .HasMaxLength(500);

            // Configure relationships
            builder.HasOne(x => x.Room)
                .WithMany()
                .HasForeignKey(x => x.RoomId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Day)
                .WithMany()
                .HasForeignKey(x => x.DayId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ReservedByUser)
                .WithMany()
                .HasForeignKey("ReservedByUserId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure many-to-many relationship with TimeSlots
            builder.HasMany(x => x.TimeRange)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "SchaduledBookingTimeSlots",
                    j => j.HasOne<UniversityBooking.TimeSlots.TimeSlot>()
                        .WithMany()
                        .HasForeignKey("TimeSlotId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<SchaduledBooking>()
                        .WithMany()
                        .HasForeignKey("SchaduledBookingId")
                        .OnDelete(DeleteBehavior.Cascade));

            // Create composite index for performance
            builder.HasIndex(x => new { x.RoomId, x.DayId, x.Status })
                .HasDatabaseName("IX_SchaduledBookings_RoomDayStatus");
        }
    }
}
