using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityBooking.TimeSlots;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace UniversityBooking.EntityFrameworkCore.Configurations;

public class SchaduledBookingConfiguration : IEntityTypeConfiguration<TimeSlot>
{
  public void Configure(EntityTypeBuilder<TimeSlot> builder)
  {

    builder.ConfigureByConvention();
  }
}

