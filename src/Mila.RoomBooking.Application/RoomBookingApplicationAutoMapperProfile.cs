using AutoMapper;
using UniversityBooking.BookingBoard;
using UniversityBooking.BookingBoard.Dtos;
using UniversityBooking.BookingRequests;
using UniversityBooking.BookingRequests.Dtos;
using UniversityBooking.Bookings;
using UniversityBooking.Bookings.Dtos;
using UniversityBooking.Days;
using UniversityBooking.Days.Dtos;
using UniversityBooking.Rooms;
using UniversityBooking.Rooms.Dtos;
using UniversityBooking.TimeSlots;
using UniversityBooking.TimeSlots.Dtos;

namespace Mila.RoomBooking;

public class RoomBookingApplicationAutoMapperProfile : Profile
{
    public RoomBookingApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        // Room
        CreateMap<Room, RoomDto>();
        CreateMap<CreateUpdateRoomDto, Room>();

        // TimeSlot
        CreateMap<TimeSlot, TimeSlotDto>();
        CreateMap<CreateUpdateTimeSlotDto, TimeSlot>();

        // Day
        CreateMap<Day, DayDto>();

        // BookingRequest
        CreateMap<BookingRequest, BookingRequestDto>()
          .ForMember(
            dest => dest.Room,
            opt => opt.MapFrom(src => src.Room))
          ;
        CreateMap<Booking, BookingDto>()
          .ForMember(
            dest => dest.Room,
            opt => opt.MapFrom(src => src.Room))
          ;
        // BookingBoard mappings
        CreateMap<WeeklyCalendar, WeeklyCalendarDto>();
        CreateMap<CalendarDayItem, CalendarDayItemDto>()
            .ForMember(
                dest => dest.FormattedDate,
                opt => opt.MapFrom(src => src.Date.ToString("MMM dd")));

        // AvailabilityAnnouncement mapping
        CreateMap<AvailabilityAnnouncement, AvailabilityAnnouncementDto>();
        CreateMap<CreateUpdateAvailabilityAnnouncementDto, AvailabilityAnnouncement>();
    }
}
