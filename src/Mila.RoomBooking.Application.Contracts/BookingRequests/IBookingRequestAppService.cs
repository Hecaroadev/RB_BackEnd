// IBookingRequestAppService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityBooking.BookingRequests.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace UniversityBooking.BookingRequests
{
    public interface IBookingRequestAppService : IApplicationService
    {
        Task<PagedResultDto<BookingRequestDto>> GetPendingRequestsAsync(PagedAndSortedResultRequestDto input);
        Task<BookingRequestDto> GetAsync(Guid id);
        Task<BookingRequestDto> CreateAsync(CreateBookingRequestDto input);
        Task<BookingRequestDto> ProcessAsync(ProcessBookingRequestDto input);
        Task<List<BookingRequestDto>> GetMyRequestsAsync(Guid? semesterId = null);
    }
}