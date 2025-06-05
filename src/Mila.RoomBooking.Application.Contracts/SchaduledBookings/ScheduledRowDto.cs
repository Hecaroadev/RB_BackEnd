using System;
using System.Collections.Generic;

namespace Mila.RoomBooking.SchaduledBookings;

public class ScheduledRowDto
{
  public Guid DayDefinitionId { get; set; }
  public string DayName { get; set; } // e.g., "Monday"
  public List<TimeSlotStateDto> TimeSlots { get; set; }

  public ScheduledRowDto()
  {
    TimeSlots = new List<TimeSlotStateDto>();
  }
}
