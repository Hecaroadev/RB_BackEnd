// IBookingRequestAppService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityBooking.BookingRequests.Dtos;
using UniversityBooking.Rooms;
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
        Task<List<BookingRequestDto>> GetMyRequestsAsync(DateTime? startDate = null, DateTime? endDate = null);
        
        /// <summary>
        /// Check if a room category is available for the specified time range
        /// </summary>
        Task<bool> IsCategoryAvailableAsync(
            RoomCategory category,
            Guid dayId,
            DateTime bookingDate,
            TimeSpan startTime,
            TimeSpan endTime,
            int requiredCapacity = 0,
            SoftwareTool requiredTools = SoftwareTool.None);
    }
}