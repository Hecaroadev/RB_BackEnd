// BookingRequestConfiguration.cs
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityBooking.BookingRequests;
using UniversityBooking.Rooms;

namespace UniversityBooking.EntityFrameworkCore.Configurations
{
    public class BookingRequestConfiguration : IEntityTypeConfiguration<BookingRequest>
    {
        public void Configure(EntityTypeBuilder<BookingRequest> builder)
        {
            builder.ToTable("BookingRequests");

            builder.Property(x => x.RoomId).IsRequired(false);
            builder.Property(x => x.RequestedBy).IsRequired().HasMaxLength(256);
            builder.Property(x => x.RequestedById).IsRequired(false);
            builder.Property(x => x.RequestDate).IsRequired();
            builder.Property(x => x.Purpose).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.RejectionReason).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.BookingDate)
              .HasColumnType("datetime2") // Use datetime2 for better precision and range
              .IsRequired(true);
            builder.Property(x => x.StartTime).IsRequired();
            builder.Property(x => x.EndTime).IsRequired();

            // New fields
            builder.Property(x => x.Category).IsRequired().HasDefaultValue(RoomCategory.Regular);
            builder.Property(x => x.InstructorName).HasMaxLength(100).HasDefaultValue(string.Empty);
            builder.Property(x => x.Subject).HasMaxLength(100).HasDefaultValue(string.Empty);
            builder.Property(x => x.NumberOfStudents).IsRequired().HasDefaultValue(0);
            // StartTime and EndTime were already configured earlier
            builder.Property(x => x.IsRecurring).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.RecurringWeeks).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.RequiredTools).IsRequired().HasDefaultValue(SoftwareTool.None);
            // Index for faster lookups
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.RequestedById);

            // Relationships
            builder
                .HasOne(br => br.Room)
                .WithMany(r => r.BookingRequests)
                .HasForeignKey(br => br.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            // Configure RoomId as optional (nullable)
            builder.HasOne(b => b.Room)
              .WithMany()
              .HasForeignKey(b => b.RoomId)
              .IsRequired(false); // This makes the relationship optional




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
