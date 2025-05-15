using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mila.RoomBooking.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class RoomBookingDbContextFactory : IDesignTimeDbContextFactory<RoomBookingDbContext>
{
    public RoomBookingDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        RoomBookingEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<RoomBookingDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new RoomBookingDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Mila.RoomBooking.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
