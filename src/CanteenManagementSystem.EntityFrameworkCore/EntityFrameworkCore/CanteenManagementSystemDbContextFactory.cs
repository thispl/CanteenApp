using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CanteenManagementSystem.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class CanteenManagementSystemDbContextFactory : IDesignTimeDbContextFactory<CanteenManagementSystemDbContext>
{
    public CanteenManagementSystemDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        CanteenManagementSystemEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<CanteenManagementSystemDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new CanteenManagementSystemDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CanteenManagementSystem.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
