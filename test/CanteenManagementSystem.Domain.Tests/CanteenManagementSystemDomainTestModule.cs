using Volo.Abp.Modularity;

namespace CanteenManagementSystem;

[DependsOn(
    typeof(CanteenManagementSystemDomainModule),
    typeof(CanteenManagementSystemTestBaseModule)
)]
public class CanteenManagementSystemDomainTestModule : AbpModule
{

}
