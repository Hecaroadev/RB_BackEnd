// IRoomAppService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityBooking.Bookings.Dtos;
using UniversityBooking.Rooms.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace UniversityBooking.Rooms
{
    public interface IRoomAppService :
        ICrudAppService<
    RoomDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateRoomDto,
    CreateUpdateRoomDto>
    {
    Task<List<BookingDto>> GetBookingsAsync(Guid id, Guid? semesterId = null);
    Task<PagedResultDto<RoomDto>> GetAvailableRoomsAsync(GetAvailableRoomsInput input);
    }

    public class GetAvailableRoomsInput : PagedAndSortedResultRequestDto
    {

    }
}
