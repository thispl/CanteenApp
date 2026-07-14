using System.Threading.Tasks;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.TenantManagement;
using CanteenManagementSystem.CanteenManagement.SyncWorker;
using CanteenManagementSystem.CanteenManagement.Services;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.AutoMapper;
using Volo.Abp;

namespace CanteenManagementSystem;

[DependsOn(
    typeof(CanteenManagementSystemDomainModule),
    typeof(CanteenManagementSystemApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpBackgroundWorkersModule)
    )]
public class CanteenManagementSystemApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Configure AutoMapper
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<CanteenManagementSystemApplicationModule>();
        });
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        // Register and start the background workers
        await context.AddBackgroundWorkerAsync<ZkDatabaseSyncWorker>();
        await context.AddBackgroundWorkerAsync<EmployeeSyncWorker>();
        await context.AddBackgroundWorkerAsync<MealTransactionSyncWorker>();
    }
}
