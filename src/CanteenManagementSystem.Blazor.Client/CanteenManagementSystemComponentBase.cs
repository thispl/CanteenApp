using CanteenManagementSystem.Localization;
using Volo.Abp.AspNetCore.Components;

namespace CanteenManagementSystem.Blazor.Client;

public abstract class CanteenManagementSystemComponentBase : AbpComponentBase
{
    protected CanteenManagementSystemComponentBase()
    {
        LocalizationResource = typeof(CanteenManagementSystemResource);
    }
}
