using Volo.Abp.Modularity;

namespace CanteenManagementSystem;

[DependsOn(
    typeof(CanteenManagementSystemApplicationModule),
    typeof(CanteenManagementSystemDomainTestModule)
)]
public class CanteenManagementSystemApplicationTestModule : AbpModule
{

}
