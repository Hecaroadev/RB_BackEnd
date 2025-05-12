using Volo.Abp.Modularity;

namespace Mila.RoomBooking;

/* Inherit from this class for your domain layer tests. */
public abstract class RoomBookingDomainTestBase<TStartupModule> : RoomBookingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
