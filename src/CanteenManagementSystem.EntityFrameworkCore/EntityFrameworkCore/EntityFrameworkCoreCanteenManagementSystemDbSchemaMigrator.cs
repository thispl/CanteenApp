using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CanteenManagementSystem.Data;
using Volo.Abp.DependencyInjection;

namespace CanteenManagementSystem.EntityFrameworkCore;

public class EntityFrameworkCoreCanteenManagementSystemDbSchemaMigrator
    : ICanteenManagementSystemDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreCanteenManagementSystemDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the CanteenManagementSystemDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<CanteenManagementSystemDbContext>()
            .Database
            .MigrateAsync();
    }
}
