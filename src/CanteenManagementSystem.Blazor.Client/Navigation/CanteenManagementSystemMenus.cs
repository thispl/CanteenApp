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

    // Admin Group
    public const string AdminGroup = Prefix + ".Admin";
    public const string BrandingSettings = AdminGroup + ".Branding";
    public const string SystemConfig = AdminGroup + ".SystemConfig";
}