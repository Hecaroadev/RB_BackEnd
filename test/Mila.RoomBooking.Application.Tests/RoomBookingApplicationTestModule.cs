using Volo.Abp.Modularity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Mila.RoomBooking.EntityFrameworkCore;
using Volo.Abp.Authorization.Permissions;


using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Volo.Abp.Uow;
using Microsoft.AspNetCore.Authorization;
using NSubstitute;

namespace Mila.RoomBooking
{
    [DependsOn(
        typeof(RoomBookingApplicationModule),
        typeof(RoomBookingEntityFrameworkCoreTestModule),
        typeof(AbpAutofacModule),
        typeof(AbpEntityFrameworkCoreSqliteModule),
        typeof(AbpIdentityDomainModule)
    )]
    public class RoomBookingApplicationTestModule : AbpModule
    {
        private SqliteConnection _sqliteConnection;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            ConfigureInMemorySqlite(context.Services);

            // Mock permission checker for tests
            context.Services.AddAlwaysAllowAuthorization();

            // Mock current user for tests
            context.Services.AddCurrentUser();
        }

        private void ConfigureInMemorySqlite(IServiceCollection services)
        {
            _sqliteConnection = CreateDatabaseAndGetConnection();

            services.Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(context =>
                {
                    context.DbContextOptions.UseSqlite(_sqliteConnection);
                });
            });
        }

        private static SqliteConnection CreateDatabaseAndGetConnection()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<RoomBookingDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new RoomBookingDbContext(options))
            {
                context.GetService<IRelationalDatabaseCreator>().CreateTables();
            }

            return connection;
        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            _sqliteConnection?.Dispose();
        }
    }
}
