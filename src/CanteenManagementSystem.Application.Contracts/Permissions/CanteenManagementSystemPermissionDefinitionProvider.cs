using CanteenManagementSystem.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace CanteenManagementSystem.Permissions;

public class CanteenManagementSystemPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(CanteenManagementSystemPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(CanteenManagementSystemPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CanteenManagementSystemResource>(name);
    }
}
