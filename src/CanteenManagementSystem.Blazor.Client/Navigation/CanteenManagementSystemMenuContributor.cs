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

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.MealTransactionMaster,
            l["Menu:MealTransactionMaster"],
            "/canteen/meal-transactions",
            icon: "fas fa-receipt",
            order: 10
        ));

        canteenManagement.AddItem(new ApplicationMenuItem(
            CanteenManagementSystemMenus.CashDepositMaster,
            l["Menu:CashDepositMaster"],
            "/canteen/cash-deposits",
            icon: "fas fa-wallet",
            order: 11
        ));

        context.Menu.AddItem(canteenManagement);

        // Reports Section
        var reportsGroup = new ApplicationMenuItem(
            CanteenManagementSystemMenus.ReportsGroup,
            l["Menu:Reports"],
            icon: "fas fa-chart-bar",
            order: 3
        );

        var dailyReports = new ApplicationMenuItem(
            CanteenManagementSystemMenus.DailyReportsGroup,
            l["Menu:DailyReports"],
            icon: "fas fa-calendar-day",
            order: 0
        );
        dailyReports.AddItem(new ApplicationMenuItem(CanteenManagementSystemMenus.DailyFoodWiseReport, l["Menu:DailyFoodWiseReport"], "/reports/daily-food-wise", icon: "fas fa-utensils", order: 0));
        dailyReports.AddItem(new ApplicationMenuItem(CanteenManagementSystemMenus.DailyEmployeeWiseReport, l["Menu:DailyEmployeeWiseReport"], "/reports/daily-employee-wise", icon: "fas fa-user", order: 1));
        dailyReports.AddItem(new ApplicationMenuItem(CanteenManagementSystemMenus.DailySummaryReport, l["Menu:DailySummaryReport"], "/reports/daily-summary", icon: "fas fa-list-alt", order: 2));
        dailyReports.AddItem(new ApplicationMenuItem(CanteenManagementSystemMenus.EmployeeDailySummaryReport, l["Menu:EmployeeDailySummaryReport"], "/reports/employee-daily-summary", icon: "fas fa-table", order: 3));
        reportsGroup.AddItem(dailyReports);

        var monthlyReports = new ApplicationMenuItem(
            CanteenManagementSystemMenus.MonthlyReportsGroup,
            l["Menu:MonthlyReports"],
            icon: "fas fa-calendar-alt",
            order: 1
        );
        monthlyReports.AddItem(new ApplicationMenuItem(CanteenManagementSystemMenus.MonthlyFoodWiseReport, l["Menu:MonthlyFoodWiseReport"], "/reports/monthly-food-wise", icon: "fas fa-utensils", order: 0));
        monthlyReports.AddItem(new ApplicationMenuItem(CanteenManagementSystemMenus.MonthlyEmployeeWiseReport, l["Menu:MonthlyEmployeeWiseReport"], "/reports/monthly-employee-wise", icon: "fas fa-users", order: 1));
        monthlyReports.AddItem(new ApplicationMenuItem(CanteenManagementSystemMenus.MonthlyDepartmentWiseReport, l["Menu:MonthlyDepartmentWiseReport"], "/reports/monthly-department-wise", icon: "fas fa-building", order: 2));
        monthlyReports.AddItem(new ApplicationMenuItem(CanteenManagementSystemMenus.EmployeeMonthlySummaryReport, l["Menu:EmployeeMonthlySummaryReport"], "/reports/employee-monthly-summary", icon: "fas fa-table", order: 3));
        monthlyReports.AddItem(new ApplicationMenuItem(CanteenManagementSystemMenus.ManualPunchReport, l["Menu:ManualPunchReport"], "/reports/manual-punch", icon: "fas fa-hand-pointer", order: 4));
        reportsGroup.AddItem(monthlyReports);

        context.Menu.AddItem(reportsGroup);

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
