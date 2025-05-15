using Volo.Abp.Modularity;
using Volo.Abp.Testing;

namespace Mila.RoomBooking;

/* Inherit from this class for your domain layer tests. */
public abstract class RoomBookingDomainTestBase<TStartupModule> : AbpIntegratedTest<RoomBookingDomainTestModule>

{

}
