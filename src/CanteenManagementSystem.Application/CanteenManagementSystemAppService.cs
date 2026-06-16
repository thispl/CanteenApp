using CanteenManagementSystem.Localization;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem;

/* Inherit your application services from this class.
 */
public abstract class CanteenManagementSystemAppService : ApplicationService
{
    protected CanteenManagementSystemAppService()
    {
        LocalizationResource = typeof(CanteenManagementSystemResource);
    }
}
