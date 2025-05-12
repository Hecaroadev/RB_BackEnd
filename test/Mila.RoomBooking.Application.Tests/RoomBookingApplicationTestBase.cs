using Volo.Abp.Modularity;

namespace Mila.RoomBooking;

public abstract class RoomBookingApplicationTestBase<TStartupModule> : RoomBookingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
