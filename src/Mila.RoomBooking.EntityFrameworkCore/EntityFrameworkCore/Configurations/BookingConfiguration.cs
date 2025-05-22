// BookingConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityBooking.Bookings;

namespace UniversityBooking.EntityFrameworkCore.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.Property(x => x.RoomId).IsRequired();
            builder.Property(x => x.TimeSlotId).IsRequired(false);
            builder.Property(x => x.DayId).IsRequired();
            builder.Property(x => x.ReservedBy).IsRequired().HasMaxLength(256);
            builder.Property(x => x.ReservedById).IsRequired();
            builder.Property(x => x.Purpose).IsRequired().HasMaxLength(500);
            builder.Property(x => x.IsRecurring).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.BookingDate)
              .HasColumnType("datetime2") // Use datetime2 for better precision and range
              .IsRequired(true); // Allow
            // Index for faster lookups
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.ReservedById);
            builder.HasIndex(x => x.BookingRequestId);
            builder.HasIndex(x => x.BookingDate);
            builder.HasIndex(x => new { x.RoomId, x.TimeSlotId, x.BookingDate, x.Status });

            // Relationships
            builder
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(b => b.TimeSlot)
                .WithMany()
                .HasForeignKey(b => b.TimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(b => b.Day)
                .WithMany()
                .HasForeignKey(b => b.DayId)
                .OnDelete(DeleteBehavior.Restrict);



            builder
                .HasOne(b => b.BookingRequest)
                .WithMany()
                .HasForeignKey(b => b.BookingRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(b => b.ReservedByUser)
                .WithMany()
                .HasForeignKey(b => b.ReservedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
