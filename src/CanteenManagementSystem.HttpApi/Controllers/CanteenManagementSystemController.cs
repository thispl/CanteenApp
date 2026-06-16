using CanteenManagementSystem.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace CanteenManagementSystem.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class CanteenManagementSystemController : AbpControllerBase
{
    protected CanteenManagementSystemController()
    {
        LocalizationResource = typeof(CanteenManagementSystemResource);
    }
}
