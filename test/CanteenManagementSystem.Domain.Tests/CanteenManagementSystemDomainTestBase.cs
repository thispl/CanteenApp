using Volo.Abp.Modularity;

namespace CanteenManagementSystem;

/* Inherit from this class for your domain layer tests. */
public abstract class CanteenManagementSystemDomainTestBase<TStartupModule> : CanteenManagementSystemTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
