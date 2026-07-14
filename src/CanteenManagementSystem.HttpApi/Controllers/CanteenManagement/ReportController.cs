using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace CanteenManagementSystem.Controllers.CanteenManagement;

/// <summary>
/// Dedicated controller for report Excel downloads.
/// ABP auto-API doesn't support file-returning endpoints, so these are explicit.
/// </summary>
[Authorize]
[Area("app")]
[Route("api/app/reports")]
public class ReportController : AbpControllerBase
{
    private readonly IReportAppService _reportAppService;

    public ReportController(IReportAppService reportAppService)
    {
        _reportAppService = reportAppService;
    }

    /// <summary>
    /// Download any report as an Excel (.xlsx) file.
    /// reportKey values: daily-food-wise | daily-employee-wise | daily-summary |
    ///   employee-daily-summary | monthly-food-wise | monthly-employee-wise |
    ///   monthly-department-wise | employee-monthly-summary | manual-punch
    /// </summary>
    [HttpGet("excel/{reportKey}")]
    public async Task<IActionResult> DownloadExcel(
        string reportKey,
        [FromQuery] DateTime? date,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? employeeId,
        [FromQuery] Guid? departmentId)
    {
        DailyReportFilterDto? dailyFilter = null;
        MonthlyReportFilterDto? monthlyFilter = null;

        bool isDaily = reportKey.StartsWith("daily") || reportKey == "employee-daily-summary";
        if (isDaily)
        {
            dailyFilter = new DailyReportFilterDto
            {
                Date = date ?? DateTime.Today,
                EmployeeId = employeeId
            };
        }
        else
        {
            monthlyFilter = new MonthlyReportFilterDto
            {
                From = from ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                To = to ?? DateTime.Today,
                EmployeeId = employeeId,
                DepartmentId = departmentId
            };
        }

        var bytes = await _reportAppService.ExportToExcelAsync(reportKey, dailyFilter, monthlyFilter);

        var fileName = $"{reportKey}_{DateTime.Today:yyyyMMdd}.xlsx";
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
