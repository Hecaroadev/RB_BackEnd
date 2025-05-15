using Volo.Abp;
using Volo.Abp.Testing;
namespace Mila.RoomBooking.EntityFrameworkCore;

public abstract class RoomBookingEntityFrameworkCoreTestBase : AbpIntegratedTest<RoomBookingEntityFrameworkCoreTestModule>
{
  protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
  {
    options.UseAutofac();
  }

}
