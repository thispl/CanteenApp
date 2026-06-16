using System.Threading.Tasks;

namespace CanteenManagementSystem.Data;

public interface ICanteenManagementSystemDbSchemaMigrator
{
    Task MigrateAsync();
}
