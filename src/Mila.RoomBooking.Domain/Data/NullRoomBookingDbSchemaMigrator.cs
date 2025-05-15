using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Mila.RoomBooking.Data;

/* This is used if database provider does't define
 * IRoomBookingDbSchemaMigrator implementation.
 */
public class NullRoomBookingDbSchemaMigrator : IRoomBookingDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
