using System.Threading.Tasks;

namespace Mila.RoomBooking.Data;

public interface IRoomBookingDbSchemaMigrator
{
    Task MigrateAsync();
}
