using Volo.Abp.Modularity;

namespace Mila.RoomBooking;

[DependsOn(
    typeof(RoomBookingApplicationModule),
    typeof(RoomBookingDomainTestModule)
)]
public class RoomBookingApplicationTestModule : AbpModule
{

}
