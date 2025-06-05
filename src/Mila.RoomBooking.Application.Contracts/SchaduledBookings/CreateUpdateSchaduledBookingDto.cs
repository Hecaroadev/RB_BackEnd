using System;
using System.Collections.Generic;
using UniversityBooking.Bookings;
using UniversityBooking.Days;
using UniversityBooking.Rooms;
using UniversityBooking.TimeSlots;
using Volo.Abp.Identity;

namespace Mila.RoomBooking.SchaduledBookings;

public class CreateUpdateSchaduledBookingDto
{
  public Guid RoomId { get; set; }
  public Guid DayId { get; set; }
  public string? Purpose { get; set; }
  public BookingStatus Status { get; set; }
  public List<TimeSlot> TimeRange { get; set; }
  public virtual Room Room { get; set; }
  public virtual Day Day { get; set; }
  public virtual IdentityUser? ReservedByUser { get; set; }
}
