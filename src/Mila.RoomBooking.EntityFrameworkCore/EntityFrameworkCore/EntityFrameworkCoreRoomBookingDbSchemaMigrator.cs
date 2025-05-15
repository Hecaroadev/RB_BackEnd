using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mila.RoomBooking.Data;
using Volo.Abp.DependencyInjection;

namespace Mila.RoomBooking.EntityFrameworkCore;

public class EntityFrameworkCoreRoomBookingDbSchemaMigrator
    : IRoomBookingDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreRoomBookingDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the RoomBookingDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<RoomBookingDbContext>()
            .Database
            .MigrateAsync();
    }
}
