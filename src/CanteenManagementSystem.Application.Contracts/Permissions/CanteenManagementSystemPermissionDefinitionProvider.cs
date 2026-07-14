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

        var categoryPermission = myGroup.AddPermission(CanteenManagementSystemPermissions.Category, L("Permission:Category"));
        categoryPermission.AddChild(CanteenManagementSystemPermissions.CategoryCreate, L("Permission:CategoryCreate"));
        categoryPermission.AddChild(CanteenManagementSystemPermissions.CategoryEdit, L("Permission:CategoryEdit"));
        categoryPermission.AddChild(CanteenManagementSystemPermissions.CategoryDelete, L("Permission:CategoryDelete"));

        var departmentPermission = myGroup.AddPermission(CanteenManagementSystemPermissions.Department, L("Permission:Department"));
        departmentPermission.AddChild(CanteenManagementSystemPermissions.DepartmentCreate, L("Permission:DepartmentCreate"));
        departmentPermission.AddChild(CanteenManagementSystemPermissions.DepartmentEdit, L("Permission:DepartmentEdit"));
        departmentPermission.AddChild(CanteenManagementSystemPermissions.DepartmentDelete, L("Permission:DepartmentDelete"));

        var itemPermission = myGroup.AddPermission(CanteenManagementSystemPermissions.Item, L("Permission:Item"));
        itemPermission.AddChild(CanteenManagementSystemPermissions.ItemCreate, L("Permission:ItemCreate"));
        itemPermission.AddChild(CanteenManagementSystemPermissions.ItemEdit, L("Permission:ItemEdit"));
        itemPermission.AddChild(CanteenManagementSystemPermissions.ItemDelete, L("Permission:ItemDelete"));

        var timeSchedulePermission = myGroup.AddPermission(CanteenManagementSystemPermissions.TimeSchedule, L("Permission:TimeSchedule"));
        timeSchedulePermission.AddChild(CanteenManagementSystemPermissions.TimeScheduleCreate, L("Permission:TimeScheduleCreate"));
        timeSchedulePermission.AddChild(CanteenManagementSystemPermissions.TimeScheduleEdit, L("Permission:TimeScheduleEdit"));
        timeSchedulePermission.AddChild(CanteenManagementSystemPermissions.TimeScheduleDelete, L("Permission:TimeScheduleDelete"));

        var designationPermission = myGroup.AddPermission(CanteenManagementSystemPermissions.Designation, L("Permission:Designation"));
        designationPermission.AddChild(CanteenManagementSystemPermissions.DesignationCreate, L("Permission:DesignationCreate"));
        designationPermission.AddChild(CanteenManagementSystemPermissions.DesignationEdit, L("Permission:DesignationEdit"));
        designationPermission.AddChild(CanteenManagementSystemPermissions.DesignationDelete, L("Permission:DesignationDelete"));

        var companyPermission = myGroup.AddPermission(CanteenManagementSystemPermissions.Company, L("Permission:Company"));
        companyPermission.AddChild(CanteenManagementSystemPermissions.CompanyCreate, L("Permission:CompanyCreate"));
        companyPermission.AddChild(CanteenManagementSystemPermissions.CompanyEdit, L("Permission:CompanyEdit"));
        companyPermission.AddChild(CanteenManagementSystemPermissions.CompanyDelete, L("Permission:CompanyDelete"));

        var devicePermission = myGroup.AddPermission(CanteenManagementSystemPermissions.Device, L("Permission:Device"));
        devicePermission.AddChild(CanteenManagementSystemPermissions.DeviceCreate, L("Permission:DeviceCreate"));
        devicePermission.AddChild(CanteenManagementSystemPermissions.DeviceEdit, L("Permission:DeviceEdit"));
        devicePermission.AddChild(CanteenManagementSystemPermissions.DeviceDelete, L("Permission:DeviceDelete"));

        var mealTransactionPermission = myGroup.AddPermission(CanteenManagementSystemPermissions.MealTransaction, L("Permission:MealTransaction"));
        mealTransactionPermission.AddChild(CanteenManagementSystemPermissions.MealTransactionCreate, L("Permission:MealTransactionCreate"));
        mealTransactionPermission.AddChild(CanteenManagementSystemPermissions.MealTransactionEdit, L("Permission:MealTransactionEdit"));
        mealTransactionPermission.AddChild(CanteenManagementSystemPermissions.MealTransactionDelete, L("Permission:MealTransactionDelete"));

        var cashDepositPermission = myGroup.AddPermission(CanteenManagementSystemPermissions.CashDeposit, L("Permission:CashDeposit"));
        cashDepositPermission.AddChild(CanteenManagementSystemPermissions.CashDepositCreate, L("Permission:CashDepositCreate"));
        cashDepositPermission.AddChild(CanteenManagementSystemPermissions.CashDepositEdit, L("Permission:CashDepositEdit"));
        cashDepositPermission.AddChild(CanteenManagementSystemPermissions.CashDepositDelete, L("Permission:CashDepositDelete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CanteenManagementSystemResource>(name);
    }
}
