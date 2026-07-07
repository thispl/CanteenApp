using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Uow;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Studio;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using CanteenManagementSystem.EntityFrameworkCore.ZkTecoIntegration;

namespace CanteenManagementSystem.EntityFrameworkCore;

[DependsOn(
    typeof(CanteenManagementSystemDomainModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule)
    )]
public class CanteenManagementSystemEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {

        CanteenManagementSystemEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<CanteenManagementSystemDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);

            // Register custom repositories (entity -> repository implementation)
            options.AddRepository<Employee, EfCoreEmployeeRepository>();
            options.AddRepository<CanteenCheckIn, EfCoreCanteenCheckInRepository>();
            options.AddRepository<SyncState, EfCoreSyncStateRepository>();
            options.AddRepository<Category, EfCoreCategoryRepository>();
            options.AddRepository<Department, EfCoreDepartmentRepository>();
            options.AddRepository<Item, EfCoreItemRepository>();
        });

        // Register ZkTeco external DbContext for read-only access
        context.Services.AddAbpDbContext<ZkTecoDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: false);
        });

        if (AbpStudioAnalyzeHelper.IsInAnalyzeMode)
        {
            return;
        }

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also CanteenManagementSystemDbContextFactory for EF Core tooling. */
            options.UseSqlServer();
        });
    }
}
