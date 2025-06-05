using System;

namespace Mila.RoomBooking.SchaduledBookings;

public class TimeSlotStateDto
{
  public Guid TimeSlotDefinitionId { get; set; }
  public TimeSpan StartTime { get; set; }
  public TimeSpan EndTime { get; set; }
  public int Order { get; set; }
  public bool IsBooked { get; set; }
  public Guid? ScheduledBookingId { get; set; } // SB_id if booked
  public string? InstructorName { get; set; }
  public string? Subject { get; set; }
}