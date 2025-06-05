using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Mila.RoomBooking.SchaduledBookings;

public interface ISchaduledBookingAppService : IApplicationService
{
  Task<WeekGridDto> GetWeekGridByRoomIdAsync(Guid roomId, DateTime weekStartDate);
  Task<SchaduledBookingDto> GetScheduledBookingByIdAsync(Guid sbId);
  Task CancelScheduledBookingBySbIdAsync(Guid sbId);
  Task<bool> CheckAvailabilityAsync(AvailabilityCheckRequestDto input);
  Task<SchaduledBookingDto> CreateScheduledBookingAsync(CreateUpdateSchaduledBookingDto input);
}
