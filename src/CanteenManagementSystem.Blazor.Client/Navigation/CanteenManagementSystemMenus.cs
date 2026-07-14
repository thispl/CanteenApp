namespace CanteenManagementSystem.Blazor.Client.Navigation;

public class CanteenManagementSystemMenus
{
    private const string Prefix = "CanteenManagementSystem";

    public const string Home = Prefix + ".Home";

    // Canteen Management Group
    public const string CanteenManagementGroup = Prefix + ".CanteenManagement";
    public const string Dashboard = CanteenManagementGroup + ".Dashboard";
    public const string EmployeeDirectory = CanteenManagementGroup + ".Employees";
    public const string LiveCanteenLogs = CanteenManagementGroup + ".CheckIns";
    public const string CategoryMaster = CanteenManagementGroup + ".Categories";
    public const string DepartmentMaster = CanteenManagementGroup + ".Departments";
    public const string ItemMaster = CanteenManagementGroup + ".Items";
    public const string TimeScheduleMaster = CanteenManagementGroup + ".TimeSchedules";
    public const string DesignationMaster = CanteenManagementGroup + ".Designations";
    public const string CompanyMaster = CanteenManagementGroup + ".Companies";
    public const string DeviceMaster = CanteenManagementGroup + ".Devices";
    public const string MealTransactionMaster = CanteenManagementGroup + ".MealTransactions";
    public const string CashDepositMaster = CanteenManagementGroup + ".CashDeposits";

    // Reports Group
    public const string ReportsGroup = Prefix + ".Reports";
    public const string DailyReportsGroup = ReportsGroup + ".Daily";
    public const string MonthlyReportsGroup = ReportsGroup + ".Monthly";

    public const string DailyFoodWiseReport = DailyReportsGroup + ".FoodWise";
    public const string DailyEmployeeWiseReport = DailyReportsGroup + ".EmployeeWise";
    public const string DailySummaryReport = DailyReportsGroup + ".Summary";
    public const string EmployeeDailySummaryReport = DailyReportsGroup + ".EmployeeSummary";

    public const string MonthlyFoodWiseReport = MonthlyReportsGroup + ".FoodWise";
    public const string MonthlyEmployeeWiseReport = MonthlyReportsGroup + ".EmployeeWise";
    public const string MonthlyDepartmentWiseReport = MonthlyReportsGroup + ".DeptWise";
    public const string EmployeeMonthlySummaryReport = MonthlyReportsGroup + ".EmployeeSummary";
    public const string ManualPunchReport = MonthlyReportsGroup + ".ManualPunch";

    // Admin Group
    public const string AdminGroup = Prefix + ".Admin";
    public const string BrandingSettings = AdminGroup + ".Branding";
    public const string SystemConfig = AdminGroup + ".SystemConfig";
}