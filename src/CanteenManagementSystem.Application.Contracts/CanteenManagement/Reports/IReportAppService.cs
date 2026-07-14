using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Reports;

/// <summary>
/// Read-only report queries. All methods return materialised lists suitable for
/// rendering in Blazor tables or serialising to Excel/CSV.
/// </summary>
public interface IReportAppService : IApplicationService
{
    // ── Daily reports ────────────────────────────────────────────────────────

    /// <summary>Report 1 – Daily Count: food-wise (count + revenue per item).</summary>
    Task<List<FoodWiseReportRowDto>> GetDailyFoodWiseAsync(DailyReportFilterDto input);

    /// <summary>Report 2 – Daily Count: employee-wise (count + revenue per employee).</summary>
    Task<List<EmployeeWiseReportRowDto>> GetDailyEmployeeWiseAsync(DailyReportFilterDto input);

    /// <summary>Report 3 – Daily Summary: per time-schedule slot (count + revenue).</summary>
    Task<List<DailySummaryReportRowDto>> GetDailySummaryAsync(DailyReportFilterDto input);

    /// <summary>Report 9 – Employee Wise Daily Summary (employee × time-schedule for a date).</summary>
    Task<List<EmployeeDailySummaryRowDto>> GetEmployeeDailySummaryAsync(DailyReportFilterDto input);

    // ── Monthly reports ──────────────────────────────────────────────────────

    /// <summary>Report 4 – Monthly Count: food-wise.</summary>
    Task<List<FoodWiseReportRowDto>> GetMonthlyFoodWiseAsync(MonthlyReportFilterDto input);

    /// <summary>Report 5 – Monthly Count: employee-wise.</summary>
    Task<List<EmployeeWiseReportRowDto>> GetMonthlyEmployeeWiseAsync(MonthlyReportFilterDto input);

    /// <summary>Report 6 – Monthly Count: department-wise.</summary>
    Task<List<DepartmentWiseReportRowDto>> GetMonthlyDepartmentWiseAsync(MonthlyReportFilterDto input);

    /// <summary>Report 8 – Employee Wise Monthly Summary (employee × time-schedule for month).</summary>
    Task<List<EmployeeMonthlyDetailRowDto>> GetEmployeeMonthlySummaryAsync(MonthlyReportFilterDto input);

    /// <summary>Report 10 – Employee Manual Punch Report (Source = ManualEntry).</summary>
    Task<List<ManualPunchReportRowDto>> GetManualPunchReportAsync(MonthlyReportFilterDto input);

    // ── Excel export ─────────────────────────────────────────────────────────

    /// <summary>Generate Excel workbook bytes for any report by key.</summary>
    Task<byte[]> ExportToExcelAsync(string reportKey, DailyReportFilterDto? dailyFilter, MonthlyReportFilterDto? monthlyFilter);
}
