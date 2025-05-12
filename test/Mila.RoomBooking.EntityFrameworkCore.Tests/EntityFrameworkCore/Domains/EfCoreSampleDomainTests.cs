using Mila.RoomBooking.Samples;
using Xunit;

namespace Mila.RoomBooking.EntityFrameworkCore.Domains;

[Collection(RoomBookingTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<RoomBookingEntityFrameworkCoreTestModule>
{

}
