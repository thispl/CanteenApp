using Volo.Abp.Modularity;

namespace CanteenManagementSystem;

public abstract class CanteenManagementSystemApplicationTestBase<TStartupModule> : CanteenManagementSystemTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
