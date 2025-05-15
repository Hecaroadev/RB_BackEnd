using System;
using System.Threading.Tasks;
using Shouldly;
using UniversityBooking.BookingRequests;
using UniversityBooking.Rooms;
using Volo.Abp;
using Volo.Abp.Identity;
using Xunit;
using Volo.Abp.Uow;
using Volo.Abp.Testing;

namespace Mila.RoomBooking.BookingRequests
{
    public class BookingRequestTests : RoomBookingTestBase<RoomBookingApplicationTestModule>
    {
        private readonly IRoomBookingManager _roomBookingManager;
        private readonly IIdentityUserRepository _userRepository;

        public BookingRequestTests()
        {
            _roomBookingManager = GetRequiredService<IRoomBookingManager>();
            _userRepository = GetRequiredService<IIdentityUserRepository>();
        }

        [Fact]
        public async Task Booking_Request_Should_Be_Submitted()
        {
            // Arrange
            var roomId = Guid.NewGuid();
            var timeSlotId = Guid.NewGuid();
            var dayId = Guid.NewGuid();
            var semesterId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var username = "testuser";
            var purpose = "Test booking purpose";

            // Create a test user
            var user = new IdentityUser(userId, username, $"{username}@example.com");

            // Act
            var bookingRequest = await WithUnitOfWorkAsync(async () =>
            {
                return await _roomBookingManager.CreateBookingRequestAsync(
                    roomId,
                    timeSlotId,
                    dayId,
                    semesterId,
                    userId,
                    username,
                    purpose,
                    user
                );
            });

            // Assert
            bookingRequest.ShouldNotBeNull();
            bookingRequest.Id.ShouldNotBe(Guid.Empty);
            bookingRequest.RoomId.ShouldBe(roomId);
            bookingRequest.TimeSlotId.ShouldBe(timeSlotId);
            bookingRequest.DayId.ShouldBe(dayId);
            bookingRequest.SemesterId.ShouldBe(semesterId);
            bookingRequest.RequestedById.ShouldBe(userId);
            bookingRequest.RequestedBy.ShouldBe(username);
            bookingRequest.Purpose.ShouldBe(purpose);
            bookingRequest.Status.ShouldBe(BookingRequestStatus.Pending);
            bookingRequest.RequestDate.Date.ShouldBe(DateTime.Now.Date);
        }
    }
}
