namespace CanteenManagementSystem.Permissions;

public static class CanteenManagementSystemPermissions
{
    public const string GroupName = "CanteenManagementSystem";

    // Category permissions
    public const string Category = GroupName + ".Category";
    public const string CategoryCreate = Category + ".Create";
    public const string CategoryEdit = Category + ".Edit";
    public const string CategoryDelete = Category + ".Delete";

    // Department permissions
    public const string Department = GroupName + ".Department";
    public const string DepartmentCreate = Department + ".Create";
    public const string DepartmentEdit = Department + ".Edit";
    public const string DepartmentDelete = Department + ".Delete";

    // Item permissions
    public const string Item = GroupName + ".Item";
    public const string ItemCreate = Item + ".Create";
    public const string ItemEdit = Item + ".Edit";
    public const string ItemDelete = Item + ".Delete";

    // Time Schedule permissions
    public const string TimeSchedule = GroupName + ".TimeSchedule";
    public const string TimeScheduleCreate = TimeSchedule + ".Create";
    public const string TimeScheduleEdit = TimeSchedule + ".Edit";
    public const string TimeScheduleDelete = TimeSchedule + ".Delete";

    // Designation permissions
    public const string Designation = GroupName + ".Designation";
    public const string DesignationCreate = Designation + ".Create";
    public const string DesignationEdit = Designation + ".Edit";
    public const string DesignationDelete = Designation + ".Delete";

    // Company permissions
    public const string Company = GroupName + ".Company";
    public const string CompanyCreate = Company + ".Create";
    public const string CompanyEdit = Company + ".Edit";
    public const string CompanyDelete = Company + ".Delete";

    // Device permissions
    public const string Device = GroupName + ".Device";
    public const string DeviceCreate = Device + ".Create";
    public const string DeviceEdit = Device + ".Edit";
    public const string DeviceDelete = Device + ".Delete";

    // Meal Transaction permissions
    public const string MealTransaction = GroupName + ".MealTransaction";
    public const string MealTransactionCreate = MealTransaction + ".Create";
    public const string MealTransactionEdit = MealTransaction + ".Edit";
    public const string MealTransactionDelete = MealTransaction + ".Delete";

    // Cash Deposit permissions
    public const string CashDeposit = GroupName + ".CashDeposit";
    public const string CashDepositCreate = CashDeposit + ".Create";
    public const string CashDepositEdit = CashDeposit + ".Edit";
    public const string CashDepositDelete = CashDeposit + ".Delete";
}
