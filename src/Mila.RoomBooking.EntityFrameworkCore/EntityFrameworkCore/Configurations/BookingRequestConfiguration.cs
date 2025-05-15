// BookingRequestConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityBooking.BookingRequests;

namespace UniversityBooking.EntityFrameworkCore.Configurations
{
    public class BookingRequestConfiguration : IEntityTypeConfiguration<BookingRequest>
    {
        public void Configure(EntityTypeBuilder<BookingRequest> builder)
        {
            builder.ToTable("BookingRequests");

            builder.Property(x => x.RoomId).IsRequired();
            builder.Property(x => x.TimeSlotId).IsRequired();
            builder.Property(x => x.DayId).IsRequired();
            builder.Property(x => x.SemesterId).IsRequired();
            builder.Property(x => x.RequestedBy).IsRequired().HasMaxLength(256);
            builder.Property(x => x.RequestedById).IsRequired();
            builder.Property(x => x.RequestDate).IsRequired();
            builder.Property(x => x.Purpose).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.RejectionReason).HasMaxLength(500).IsRequired(false);

            // Index for faster lookups
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.RequestedById);
            builder.HasIndex(x => new { x.RoomId, x.TimeSlotId, x.DayId, x.SemesterId, x.Status });

            // Relationships
            builder
                .HasOne(br => br.Room)
                .WithMany(r => r.BookingRequests)
                .HasForeignKey(br => br.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(br => br.TimeSlot)
                .WithMany()
                .HasForeignKey(br => br.TimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(br => br.Day)
                .WithMany()
                .HasForeignKey(br => br.DayId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(br => br.Semester)
                .WithMany()
                .HasForeignKey(br => br.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(br => br.RequestedByUser)
                .WithMany()
                .HasForeignKey(br => br.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(br => br.ProcessedByUser)
                .WithMany()
                .HasForeignKey(br => br.ProcessedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
