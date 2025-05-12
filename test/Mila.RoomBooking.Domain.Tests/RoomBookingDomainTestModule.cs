using Volo.Abp.Modularity;

namespace Mila.RoomBooking;

[DependsOn(
    typeof(RoomBookingDomainModule),
    typeof(RoomBookingTestBaseModule)
)]
public class RoomBookingDomainTestModule : AbpModule
{

}
