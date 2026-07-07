using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CanteenManagementSystem.Localization;
using CanteenManagementSystem.Permissions;
using CanteenManagementSystem.MultiTenancy;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.Users;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.Identity.Blazor;

namespace CanteenManagementSystem.Blazor.Client.Navigation;

public class CanteenManagementSystemMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public CanteenManagementSystemMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<CanteenManagementSystemResource>();
        
        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        context.Menu.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.Home,
            l["Menu:Home"],
            "/",
            icon: "fas fa-home",
            order: 1
        ));

        // Canteen Management Section
        var canteenManagement = new ApplicationMenuItem(
            CanteenManagementSystemMenus.CanteenManagementGroup,
            l["Menu:CanteenManagement"],
            icon: "fas fa-utensils",
            order: 2
        );

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.Dashboard,
            l["Menu:Dashboard"],
            "/canteen/dashboard",
            icon: "fas fa-chart-line",
            order: 0
        ));

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.EmployeeDirectory,
            l["Menu:EmployeeDirectory"],
            "/canteen/employees",
            icon: "fas fa-users",
            order: 1
        ));

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.LiveCanteenLogs,
            l["Menu:LiveCanteenLogs"],
            "/canteen/checkins",
            icon: "fas fa-clock",
            order: 2
        ));

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.CategoryMaster,
            l["Menu:CategoryMaster"],
            "/canteen/categories",
            icon: "fas fa-tags",
            order: 3
        ).RequirePermissions(CanteenManagementSystemPermissions.Category));

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.DepartmentMaster,
            l["Menu:DepartmentMaster"],
            "/canteen/departments",
            icon: "fas fa-building",
            order: 4
        ).RequirePermissions(CanteenManagementSystemPermissions.Department));

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.ItemMaster,
            l["Menu:ItemMaster"],
            "/canteen/items",
            icon: "fas fa-utensils",
            order: 5
        ).RequirePermissions(CanteenManagementSystemPermissions.Item));

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.TimeScheduleMaster,
            l["Menu:TimeScheduleMaster"],
            "/canteen/time-schedules",
            icon: "fas fa-clock",
            order: 6
        ).RequirePermissions(CanteenManagementSystemPermissions.TimeSchedule));

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.DesignationMaster,
            l["Menu:DesignationMaster"],
            "/canteen/designations",
            icon: "fas fa-id-badge",
            order: 7
        ).RequirePermissions(CanteenManagementSystemPermissions.Designation));

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.CompanyMaster,
            l["Menu:CompanyMaster"],
            "/canteen/companies",
            icon: "fas fa-building",
            order: 8
        ).RequirePermissions(CanteenManagementSystemPermissions.Company));

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.DeviceMaster,
            l["Menu:DeviceMaster"],
            "/canteen/devices",
            icon: "fas fa-microchip",
            order: 9
        ).RequirePermissions(CanteenManagementSystemPermissions.Device));

        context.Menu.AddItem(canteenManagement);

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);

        // Add Branding Settings to Administration
        administration.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.BrandingSettings,
            l["Menu:BrandingSettings"],
            "/admin/branding",
            icon: "fas fa-palette",
            order: 100
        ).RequireAuthenticated());

        // Add System Config to Administration
        administration.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.SystemConfig,
            "System Configuration",
            "/admin/system-config",
            icon: "fas fa-server",
            order: 101
        ).RequireAuthenticated());
    }

    private async Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var accountStringLocalizer = context.GetLocalizer<AccountResource>();
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "";

        context.Menu.AddItem(new ApplicationMenuItem(
            "Account.Manage",
            accountStringLocalizer["MyAccount"],
            $"{authServerUrl.EnsureEndsWith('/')}Account/Manage",
            icon: "fa fa-cog",
            order: 1000,
            target: "_blank").RequireAuthenticated());

        await Task.CompletedTask;
    }
}
