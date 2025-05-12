using Mila.RoomBooking.Samples;
using Xunit;

namespace Mila.RoomBooking.EntityFrameworkCore.Applications;

[Collection(RoomBookingTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<RoomBookingEntityFrameworkCoreTestModule>
{

}
