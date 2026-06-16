using CanteenManagementSystem.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace CanteenManagementSystem.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(CanteenManagementSystemEntityFrameworkCoreModule),
    typeof(CanteenManagementSystemApplicationContractsModule)
)]
public class CanteenManagementSystemDbMigratorModule : AbpModule
{
}
