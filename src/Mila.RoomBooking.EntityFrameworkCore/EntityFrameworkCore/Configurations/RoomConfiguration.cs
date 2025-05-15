// RoomConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityBooking.Rooms;

namespace UniversityBooking.EntityFrameworkCore.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");

            builder.Property(x => x.Number).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Building).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Floor).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Capacity).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

            // Index for faster lookups
            builder.HasIndex(x => x.Number);
            builder.HasIndex(x => x.Building);
            builder.HasIndex(x => new { x.Building, x.Number }).IsUnique();

            // Relationships
            builder
                .HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(r => r.BookingRequests)
                .WithOne(br => br.Room)
                .HasForeignKey(br => br.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}