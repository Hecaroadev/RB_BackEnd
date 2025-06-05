using System;

namespace Mila.RoomBooking.SchaduledBookings;

public class AvailabilityCheckRequestDto
{
  public Guid RoomId { get; set; }
  public DateTime BookingDate { get; set; }
  public TimeSpan TimeRangeStart { get; set; }
  public TimeSpan TimeRangeEnd { get; set; }
  public Guid? ExcludeBookingId { get; set; } // Optional: For checking availability when editing an existing booking
}