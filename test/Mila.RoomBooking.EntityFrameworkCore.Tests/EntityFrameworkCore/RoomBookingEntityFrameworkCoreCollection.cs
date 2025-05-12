using Xunit;

namespace Mila.RoomBooking.EntityFrameworkCore;

[CollectionDefinition(RoomBookingTestConsts.CollectionDefinitionName)]
public class RoomBookingEntityFrameworkCoreCollection : ICollectionFixture<RoomBookingEntityFrameworkCoreFixture>
{

}
