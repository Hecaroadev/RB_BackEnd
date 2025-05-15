using System;
using System.Threading.Tasks;
using UniversityBooking.BookingBoard.Dtos;
using Volo.Abp.Application.Services;

namespace UniversityBooking.BookingBoard
{
    public interface IBookingBoardAppService : IApplicationService
    {
        /// <summary>
        /// Gets the booking board data for the current week
        /// </summary>
        Task<BookingBoardDto> GetCurrentWeekAsync(Guid? roomId = null);
        
        /// <summary>
        /// Gets the booking board data for a specific date
        /// </summary>
        Task<BookingBoardDto> GetForDateAsync(DateTime date, Guid? roomId = null);
        
        /// <summary>
        /// Gets the booking board data for a specific week
        /// </summary>
        Task<BookingBoardDto> GetWeekAsync(DateTime weekStartDate, Guid? roomId = null);
        
        /// <summary>
        /// Checks if a room is available on a specific date and time slot
        /// </summary>
        Task<bool> IsRoomAvailableAsync(Guid roomId, Guid timeSlotId, DateTime date);
    }
}