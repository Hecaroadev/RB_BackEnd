using System;
using System.Threading.Tasks;
using Shouldly;
using UniversityBooking.BookingRequests;
using UniversityBooking.BookingRequests.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Xunit;

namespace Mila.RoomBooking.BookingRequests
{
    public class BookingRequestAppServiceTests : RoomBookingApplicationTestBase
    {
        private readonly IBookingRequestAppService _bookingRequestAppService;
        private readonly IRepository<BookingRequest, Guid> _bookingRequestRepository;

        public BookingRequestAppServiceTests()
        {
            _bookingRequestAppService = GetRequiredService<IBookingRequestAppService>();
            _bookingRequestRepository = GetRequiredService<IRepository<BookingRequest, Guid>>();
        }

        [Fact]
        public async Task Should_Create_Booking_Request()
        {
            // Arrange
            var createBookingDto = new CreateBookingRequestDto
            {
                RoomId = Guid.NewGuid(),
                TimeSlotId = Guid.NewGuid(),
                DayId = Guid.NewGuid(),
                SemesterId = Guid.NewGuid(),
                Purpose = "Test booking purpose",
                RequestedDate = DateTime.Now.AddDays(1)
            };

            // Mock the availability check in the test environment
            // Note: In a real test, you might want to set up test data and repositories

            // Act
            var result = await _bookingRequestAppService.CreateAsync(createBookingDto);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.RoomId.ShouldBe(createBookingDto.RoomId);
            result.TimeSlotId.ShouldBe(createBookingDto.TimeSlotId);
            result.DayId.ShouldBe(createBookingDto.DayId);
            result.SemesterId.ShouldBe(createBookingDto.SemesterId);
            result.Purpose.ShouldBe(createBookingDto.Purpose);
            result.Status.ShouldBe(BookingRequestStatus.Pending);
            
            // Verify the booking request was actually saved
            var savedRequest = await _bookingRequestRepository.FindAsync(result.Id);
            savedRequest.ShouldNotBeNull();
            savedRequest.Purpose.ShouldBe(createBookingDto.Purpose);
        }
    }
}