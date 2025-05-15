using System;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using UniversityBooking.BookingRequests;
using UniversityBooking.BookingRequests.Dtos;
using UniversityBooking.Rooms;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Xunit;

namespace Mila.RoomBooking.BookingRequests
{
    public class BookingRequestAppServiceTests : RoomBookingApplicationTestBase
    {
        private readonly IBookingRequestAppService _bookingRequestAppService;
        private readonly IRepository<BookingRequest, Guid> _bookingRequestRepository;
        private readonly IRoomBookingManager _roomBookingManager;
        private readonly ICurrentUser _currentUser;
        private readonly IIdentityUserRepository _userRepository;

        public BookingRequestAppServiceTests()
        {
            // Set up real services where possible
            _bookingRequestRepository = GetRequiredService<IRepository<BookingRequest, Guid>>();

            // Mock dependencies
            _roomBookingManager = Substitute.For<IRoomBookingManager>();
            _currentUser = Substitute.For<ICurrentUser>();
            _currentUser.Id.Returns(Guid.NewGuid());
            _currentUser.UserName.Returns("testuser");
            _currentUser.IsAuthenticated.Returns(true);

            _userRepository = Substitute.For<IIdentityUserRepository>();
            var fakeUser = new IdentityUser(Guid.NewGuid(), "testuser", "test@example.com");
            _userRepository.GetAsync(_currentUser.Id.Value).Returns(fakeUser);

            // Set up room manager to return availability for all rooms
            _roomBookingManager.IsRoomAvailableAsync(
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>())
                .Returns(true);

            _roomBookingManager
                .CreateBookingRequestAsync(
                    Arg.Any<Guid>(),
                    Arg.Any<Guid>(),
                    Arg.Any<Guid>(),
                    Arg.Any<Guid>(),
                    Arg.Any<Guid>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<IdentityUser>(),
                    Arg.Any<DateTime?>())
                .Returns(callInfo =>
                {
                    var bookingRequest = new BookingRequest(
                        Guid.NewGuid(),
                        callInfo.ArgAt<Guid>(0),
                        callInfo.ArgAt<Guid>(1),
                        callInfo.ArgAt<Guid>(2),
                        callInfo.ArgAt<Guid>(3),
                        callInfo.ArgAt<Guid>(4),
                        callInfo.ArgAt<string>(5),
                        callInfo.ArgAt<string>(6),
                        callInfo.ArgAt<IdentityUser>(7),
                        callInfo.ArgAt<DateTime?>(8)
                    );

                    return bookingRequest;
                });

            // Create the service to test with our mocks
            _bookingRequestAppService = new UniversityBooking.BookingRequests.BookingRequestAppService(
                _bookingRequestRepository,
                _roomBookingManager,
                GetRequiredService<IRepository<UniversityBooking.Bookings.Booking, Guid>>(),
                _currentUser,
                _userRepository
            );
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

            // Verify the roomBookingManager was called with correct parameters
            await _roomBookingManager.Received(1).IsRoomAvailableAsync(
                createBookingDto.RoomId,
                createBookingDto.TimeSlotId,
                createBookingDto.DayId,
                createBookingDto.SemesterId
            );

            await _roomBookingManager.Received(1).CreateBookingRequestAsync(
                Arg.Is<Guid>(x => x == createBookingDto.RoomId),
                Arg.Is<Guid>(x => x == createBookingDto.TimeSlotId),
                Arg.Is<Guid>(x => x == createBookingDto.DayId),
                Arg.Is<Guid>(x => x == createBookingDto.SemesterId),
                Arg.Is<Guid>(x => x == _currentUser.Id),
                Arg.Is<string>(x => x == _currentUser.UserName),
                Arg.Is<string>(x => x == createBookingDto.Purpose),
                Arg.Any<IdentityUser>(),
                Arg.Is<DateTime?>(x => x == createBookingDto.RequestedDate)
            );
        }
    }
}
