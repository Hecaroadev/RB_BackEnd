using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Mila.RoomBooking.SchaduledBookings;

public class WeekGridDto
{
  public Guid RoomId { get; set; }
  public List<ScheduledRowDto> ScheduledRows { get; set; }

  public WeekGridDto()
  {
    ScheduledRows = new List<ScheduledRowDto>();
  }
}

// File: YourProject.Application.Contracts/Scheduling/Dtos/AvailabilityCheckRequestDto.cs
// File: YourProject.Application.Contracts/Scheduling/Dtos/ScheduledRowDto.cs
