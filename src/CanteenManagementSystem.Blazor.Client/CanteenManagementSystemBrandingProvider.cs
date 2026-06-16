using Microsoft.Extensions.Localization;
using CanteenManagementSystem.Localization;
using Microsoft.Extensions.Localization;
using CanteenManagementSystem.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace CanteenManagementSystem.Blazor.Client;

[Dependency(ReplaceServices = true)]
public class CanteenManagementSystemBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<CanteenManagementSystemResource> _localizer;

    public CanteenManagementSystemBrandingProvider(IStringLocalizer<CanteenManagementSystemResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => "CMS";
}
