// IBookingAppService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityBooking.Bookings.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace UniversityBooking.Bookings
{
    public interface IBookingAppService : IApplicationService
    {
        Task<BookingDto> GetAsync(Guid id);
        Task<PagedResultDto<BookingDto>> GetListAsync(PagedAndSortedResultRequestDto input);
        Task<List<BookingDto>> GetByRoomAsync(Guid roomId, Guid? semesterId = null);
        Task<List<BookingDto>> GetMyBookingsAsync(Guid? semesterId = null);
        Task CancelAsync(Guid id, string reason);
    }
}