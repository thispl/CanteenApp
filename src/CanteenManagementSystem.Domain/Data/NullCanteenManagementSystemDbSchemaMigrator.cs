using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace CanteenManagementSystem.Data;

/* This is used if database provider does't define
 * ICanteenManagementSystemDbSchemaMigrator implementation.
 */
public class NullCanteenManagementSystemDbSchemaMigrator : ICanteenManagementSystemDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
