using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityBooking.Rooms;

namespace UniversityBooking.EntityFrameworkCore.Configurations
{
    public class AvailabilityAnnouncementConfiguration : IEntityTypeConfiguration<UniversityBooking.Rooms.AvailabilityAnnouncement>
    {
        public void Configure(EntityTypeBuilder<UniversityBooking.Rooms.AvailabilityAnnouncement> builder)
        {
            builder.ToTable("AvailabilityAnnouncements");
            
            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.EndDate).IsRequired();
            builder.Property(x => x.Category).IsRequired();
            builder.Property(x => x.AvailableTools).IsRequired().HasDefaultValue(SoftwareTool.None);
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
            
            // Indexes for faster lookup
            builder.HasIndex(x => x.IsActive);
            builder.HasIndex(x => x.Category);
            builder.HasIndex(x => x.StartDate);
            builder.HasIndex(x => x.EndDate);
        }
    }
}