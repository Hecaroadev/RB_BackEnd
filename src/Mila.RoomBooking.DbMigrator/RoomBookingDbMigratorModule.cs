using Mila.RoomBooking.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Mila.RoomBooking.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(RoomBookingEntityFrameworkCoreModule),
    typeof(RoomBookingApplicationContractsModule)
)]
public class RoomBookingDbMigratorModule : AbpModule
{
}
