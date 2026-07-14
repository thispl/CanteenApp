using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement;
using CanteenManagementSystem.CanteenManagement.Reports;
using CanteenManagementSystem.EntityFrameworkCore;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace CanteenManagementSystem.CanteenManagement.Reports;

[Authorize]
public class ReportAppService : ApplicationService, IReportAppService
{
    private readonly IDbContextProvider<CanteenManagementSystemDbContext> _dbContextProvider;

    public ReportAppService(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static DateTime DayStart(DateTime d) => d.Date;
    private static DateTime DayEnd(DateTime d) => d.Date.AddDays(1).AddTicks(-1);
    private static DateTime RangeEnd(DateTime d) => d.Date.AddDays(1).AddTicks(-1);

    // ═════════════════════════════════════════════════════════════════════════
    // DAILY REPORTS
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>Report 1 – Daily Count: food-wise.</summary>
    public async Task<List<FoodWiseReportRowDto>> GetDailyFoodWiseAsync(DailyReportFilterDto input)
    {
        var db = await _dbContextProvider.GetDbContextAsync();
        var from = DayStart(input.Date);
        var to = DayEnd(input.Date);

        var rows = await db.MealTransactions
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.PunchTime >= from && t.PunchTime <= to)
            .GroupBy(t => new { t.ItemId, t.Price })
            .Select(g => new
            {
                g.Key.ItemId,
                g.Key.Price,
                Count = g.Count(),
                Total = g.Sum(x => x.Price)
            })
            .ToListAsync();

        // enrich with item description
        var itemIds = rows.Select(r => r.ItemId).Distinct().ToList();
        var items = await db.Items.AsNoTracking()
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => i.Description);

        return rows
            .OrderByDescending(r => r.Count)
            .Select(r => new FoodWiseReportRowDto
            {
                ItemDescription = items.TryGetValue(r.ItemId, out var desc) ? desc : r.ItemId.ToString(),
                UnitPrice = r.Price,
                TransactionCount = r.Count,
                TotalAmount = r.Total
            })
            .ToList();
    }

    /// <summary>Report 2 – Daily Count: employee-wise.</summary>
    public async Task<List<EmployeeWiseReportRowDto>> GetDailyEmployeeWiseAsync(DailyReportFilterDto input)
    {
        var db = await _dbContextProvider.GetDbContextAsync();
        var from = DayStart(input.Date);
        var to = DayEnd(input.Date);

        var query = db.MealTransactions
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.PunchTime >= from && t.PunchTime <= to);

        if (input.EmployeeId.HasValue)
            query = query.Where(t => t.EmployeeId == input.EmployeeId.Value);

        var rows = await query
            .GroupBy(t => t.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                Count = g.Count(),
                Total = g.Sum(x => x.Price)
            })
            .ToListAsync();

        var empIds = rows.Select(r => r.EmployeeId).Distinct().ToList();
        var employees = await db.Employees.AsNoTracking()
            .Include(e => e.Department)
            .Where(e => empIds.Contains(e.Id))
            .ToListAsync();
        var empMap = employees.ToDictionary(e => e.Id);

        return rows
            .OrderByDescending(r => r.Count)
            .Select(r =>
            {
                empMap.TryGetValue(r.EmployeeId, out var emp);
                return new EmployeeWiseReportRowDto
                {
                    EmployeeIdNumber = emp?.EmployeeId ?? r.EmployeeId.ToString(),
                    EmployeeName = emp?.FullName ?? "Unknown",
                    Department = emp?.Department?.Name ?? "-",
                    TransactionCount = r.Count,
                    TotalAmount = r.Total
                };
            })
            .ToList();
    }

    /// <summary>Report 3 – Daily Summary: per time-schedule.</summary>
    public async Task<List<DailySummaryReportRowDto>> GetDailySummaryAsync(DailyReportFilterDto input)
    {
        var db = await _dbContextProvider.GetDbContextAsync();
        var from = DayStart(input.Date);
        var to = DayEnd(input.Date);

        var rows = await db.MealTransactions
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.PunchTime >= from && t.PunchTime <= to)
            .GroupBy(t => t.TimeScheduleId)
            .Select(g => new
            {
                TimeScheduleId = g.Key,
                Count = g.Count(),
                Total = g.Sum(x => x.Price)
            })
            .ToListAsync();

        var tsIds = rows.Select(r => r.TimeScheduleId).Distinct().ToList();
        var schedules = await db.TimeSchedules.AsNoTracking()
            .Where(s => tsIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id);

        return rows
            .Select(r =>
            {
                schedules.TryGetValue(r.TimeScheduleId, out var ts);
                var slot = ts != null
                    ? $"{ts.StartTime:hh\\:mm} – {ts.EndTime:hh\\:mm}"
                    : "-";
                return new DailySummaryReportRowDto
                {
                    TimeScheduleName = ts?.Name ?? r.TimeScheduleId.ToString(),
                    TimeSlot = slot,
                    TransactionCount = r.Count,
                    TotalAmount = r.Total
                };
            })
            .OrderBy(r => r.TimeScheduleName)
            .ToList();
    }

    /// <summary>Report 9 – Employee Wise Daily Summary.</summary>
    public async Task<List<EmployeeDailySummaryRowDto>> GetEmployeeDailySummaryAsync(DailyReportFilterDto input)
    {
        var db = await _dbContextProvider.GetDbContextAsync();
        var from = DayStart(input.Date);
        var to = DayEnd(input.Date);

        var query = db.MealTransactions
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.PunchTime >= from && t.PunchTime <= to);

        if (input.EmployeeId.HasValue)
            query = query.Where(t => t.EmployeeId == input.EmployeeId.Value);

        var rows = await query
            .GroupBy(t => new { t.EmployeeId, t.TimeScheduleId })
            .Select(g => new
            {
                g.Key.EmployeeId,
                g.Key.TimeScheduleId,
                Count = g.Count(),
                Total = g.Sum(x => x.Price)
            })
            .ToListAsync();

        var empIds = rows.Select(r => r.EmployeeId).Distinct().ToList();
        var tsIds = rows.Select(r => r.TimeScheduleId).Distinct().ToList();

        var employees = await db.Employees.AsNoTracking()
            .Include(e => e.Department)
            .Where(e => empIds.Contains(e.Id))
            .ToListAsync();
        var empMap = employees.ToDictionary(e => e.Id);

        var schedules = await db.TimeSchedules.AsNoTracking()
            .Where(s => tsIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => s.Name);

        return rows
            .Select(r =>
            {
                empMap.TryGetValue(r.EmployeeId, out var emp);
                schedules.TryGetValue(r.TimeScheduleId, out var tsName);
                return new EmployeeDailySummaryRowDto
                {
                    EmployeeIdNumber = emp?.EmployeeId ?? r.EmployeeId.ToString(),
                    EmployeeName = emp?.FullName ?? "Unknown",
                    Department = emp?.Department?.Name ?? "-",
                    TimeScheduleName = tsName ?? r.TimeScheduleId.ToString(),
                    TransactionCount = r.Count,
                    TotalAmount = r.Total
                };
            })
            .OrderBy(r => r.EmployeeName).ThenBy(r => r.TimeScheduleName)
            .ToList();
    }

    // ═════════════════════════════════════════════════════════════════════════
    // MONTHLY REPORTS
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>Report 4 – Monthly Count: food-wise.</summary>
    public async Task<List<FoodWiseReportRowDto>> GetMonthlyFoodWiseAsync(MonthlyReportFilterDto input)
    {
        var db = await _dbContextProvider.GetDbContextAsync();
        var from = DayStart(input.From);
        var to = RangeEnd(input.To);

        var rows = await db.MealTransactions
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.PunchTime >= from && t.PunchTime <= to)
            .GroupBy(t => new { t.ItemId, t.Price })
            .Select(g => new
            {
                g.Key.ItemId,
                g.Key.Price,
                Count = g.Count(),
                Total = g.Sum(x => x.Price)
            })
            .ToListAsync();

        var itemIds = rows.Select(r => r.ItemId).Distinct().ToList();
        var items = await db.Items.AsNoTracking()
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => i.Description);

        return rows
            .OrderByDescending(r => r.Count)
            .Select(r => new FoodWiseReportRowDto
            {
                ItemDescription = items.TryGetValue(r.ItemId, out var desc) ? desc : r.ItemId.ToString(),
                UnitPrice = r.Price,
                TransactionCount = r.Count,
                TotalAmount = r.Total
            })
            .ToList();
    }

    /// <summary>Report 5 – Monthly Count: employee-wise.</summary>
    public async Task<List<EmployeeWiseReportRowDto>> GetMonthlyEmployeeWiseAsync(MonthlyReportFilterDto input)
    {
        var db = await _dbContextProvider.GetDbContextAsync();
        var from = DayStart(input.From);
        var to = RangeEnd(input.To);

        var query = db.MealTransactions
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.PunchTime >= from && t.PunchTime <= to);

        if (input.EmployeeId.HasValue)
            query = query.Where(t => t.EmployeeId == input.EmployeeId.Value);

        var rows = await query
            .GroupBy(t => t.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                Count = g.Count(),
                Total = g.Sum(x => x.Price)
            })
            .ToListAsync();

        var empIds = rows.Select(r => r.EmployeeId).Distinct().ToList();
        var employees = await db.Employees.AsNoTracking()
            .Include(e => e.Department)
            .Where(e => empIds.Contains(e.Id))
            .ToListAsync();
        var empMap = employees.ToDictionary(e => e.Id);

        return rows
            .OrderByDescending(r => r.Count)
            .Select(r =>
            {
                empMap.TryGetValue(r.EmployeeId, out var emp);
                return new EmployeeWiseReportRowDto
                {
                    EmployeeIdNumber = emp?.EmployeeId ?? r.EmployeeId.ToString(),
                    EmployeeName = emp?.FullName ?? "Unknown",
                    Department = emp?.Department?.Name ?? "-",
                    TransactionCount = r.Count,
                    TotalAmount = r.Total
                };
            })
            .ToList();
    }

    /// <summary>Report 6 – Monthly Count: department-wise.</summary>
    public async Task<List<DepartmentWiseReportRowDto>> GetMonthlyDepartmentWiseAsync(MonthlyReportFilterDto input)
    {
        var db = await _dbContextProvider.GetDbContextAsync();
        var rangeFrom = DayStart(input.From);
        var rangeTo = RangeEnd(input.To);

        // Join MealTransaction → Employee → Department in EF — no GroupBy on nav props,
        // instead join manually so EF can translate the query to SQL.
        var joined = await (
            from t in db.MealTransactions.AsNoTracking()
            join e in db.Employees.AsNoTracking() on t.EmployeeId equals e.Id
            join d in db.Departments.AsNoTracking() on e.DepartmentId equals d.Id into dj
            from dept in dj.DefaultIfEmpty()
            where !t.IsDeleted && t.PunchTime >= rangeFrom && t.PunchTime <= rangeTo
            group t by new { DeptId = dept != null ? dept.Id : (Guid?)null, DeptName = dept != null ? dept.Name : "No Department", CCCode = dept != null ? dept.CCCode : null }
            into g
            select new
            {
                g.Key.DeptName,
                g.Key.CCCode,
                Count = g.Count(),
                Total = g.Sum(x => x.Price)
            }
        ).ToListAsync();

        return joined
            .OrderByDescending(r => r.Count)
            .Select(r => new DepartmentWiseReportRowDto
            {
                DepartmentName = r.DeptName,
                CCCode = r.CCCode ?? "-",
                TransactionCount = r.Count,
                TotalAmount = r.Total
            })
            .ToList();
    }

    /// <summary>Report 8 – Employee Wise Monthly Summary (employee × time-schedule).</summary>
    public async Task<List<EmployeeMonthlyDetailRowDto>> GetEmployeeMonthlySummaryAsync(MonthlyReportFilterDto input)
    {
        var db = await _dbContextProvider.GetDbContextAsync();
        var from = DayStart(input.From);
        var to = RangeEnd(input.To);

        var query = db.MealTransactions
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.PunchTime >= from && t.PunchTime <= to);

        if (input.EmployeeId.HasValue)
            query = query.Where(t => t.EmployeeId == input.EmployeeId.Value);

        var rows = await query
            .GroupBy(t => new { t.EmployeeId, t.TimeScheduleId })
            .Select(g => new
            {
                g.Key.EmployeeId,
                g.Key.TimeScheduleId,
                Count = g.Count(),
                Total = g.Sum(x => x.Price)
            })
            .ToListAsync();

        var empIds = rows.Select(r => r.EmployeeId).Distinct().ToList();
        var tsIds = rows.Select(r => r.TimeScheduleId).Distinct().ToList();

        var employees = await db.Employees.AsNoTracking()
            .Include(e => e.Department)
            .Where(e => empIds.Contains(e.Id))
            .ToListAsync();
        var empMap = employees.ToDictionary(e => e.Id);

        var schedules = await db.TimeSchedules.AsNoTracking()
            .Where(s => tsIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => s.Name);

        return rows
            .Select(r =>
            {
                empMap.TryGetValue(r.EmployeeId, out var emp);
                schedules.TryGetValue(r.TimeScheduleId, out var tsName);
                return new EmployeeMonthlyDetailRowDto
                {
                    EmployeeIdNumber = emp?.EmployeeId ?? r.EmployeeId.ToString(),
                    EmployeeName = emp?.FullName ?? "Unknown",
                    Department = emp?.Department?.Name ?? "-",
                    TimeScheduleName = tsName ?? r.TimeScheduleId.ToString(),
                    TransactionCount = r.Count,
                    TotalAmount = r.Total
                };
            })
            .OrderBy(r => r.EmployeeName).ThenBy(r => r.TimeScheduleName)
            .ToList();
    }

    /// <summary>Report 10 – Employee Manual Punch Report.</summary>
    public async Task<List<ManualPunchReportRowDto>> GetManualPunchReportAsync(MonthlyReportFilterDto input)
    {
        var db = await _dbContextProvider.GetDbContextAsync();
        var from = DayStart(input.From);
        var to = RangeEnd(input.To);

        var query = db.MealTransactions
            .AsNoTracking()
            .Where(t => !t.IsDeleted
                        && t.Source == MealTransactionSource.ManualEntry
                        && t.PunchTime >= from && t.PunchTime <= to);

        if (input.EmployeeId.HasValue)
            query = query.Where(t => t.EmployeeId == input.EmployeeId.Value);

        var transactions = await query
            .OrderBy(t => t.PunchTime)
            .ToListAsync();

        var empIds = transactions.Select(t => t.EmployeeId).Distinct().ToList();
        var tsIds = transactions.Select(t => t.TimeScheduleId).Distinct().ToList();
        var itemIds = transactions.Select(t => t.ItemId).Distinct().ToList();

        var employees = await db.Employees.AsNoTracking()
            .Include(e => e.Department)
            .Where(e => empIds.Contains(e.Id))
            .ToListAsync();
        var empMap = employees.ToDictionary(e => e.Id);

        var schedules = await db.TimeSchedules.AsNoTracking()
            .Where(s => tsIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => s.Name);

        var items = await db.Items.AsNoTracking()
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => i.Description);

        return transactions
            .Select(t =>
            {
                empMap.TryGetValue(t.EmployeeId, out var emp);
                schedules.TryGetValue(t.TimeScheduleId, out var tsName);
                items.TryGetValue(t.ItemId, out var itemDesc);
                return new ManualPunchReportRowDto
                {
                    EmployeeIdNumber = emp?.EmployeeId ?? t.EmployeeId.ToString(),
                    EmployeeName = emp?.FullName ?? "Unknown",
                    Department = emp?.Department?.Name ?? "-",
                    PunchTime = t.PunchTime,
                    TimeScheduleName = tsName ?? t.TimeScheduleId.ToString(),
                    ItemDescription = itemDesc ?? t.ItemId.ToString(),
                    Price = t.Price
                };
            })
            .ToList();
    }

    // ═════════════════════════════════════════════════════════════════════════
    // EXCEL EXPORT
    // ═════════════════════════════════════════════════════════════════════════

    public async Task<byte[]> ExportToExcelAsync(
        string reportKey,
        DailyReportFilterDto? dailyFilter,
        MonthlyReportFilterDto? monthlyFilter)
    {
        using var workbook = new XLWorkbook();

        switch (reportKey)
        {
            case "daily-food-wise":
            {
                var data = await GetDailyFoodWiseAsync(dailyFilter!);
                var ws = workbook.Worksheets.Add("Daily Food Wise");
                WriteHeader(ws, new[] { "Item", "Unit Price (₹)", "Count", "Total Amount (₹)" });
                int row = 2;
                foreach (var r in data)
                {
                    ws.Cell(row, 1).Value = r.ItemDescription;
                    ws.Cell(row, 2).Value = r.UnitPrice;
                    ws.Cell(row, 3).Value = r.TransactionCount;
                    ws.Cell(row, 4).Value = r.TotalAmount;
                    row++;
                }
                AddTotalsRow(ws, row, new[] { 3, 4 });
                FormatSheet(ws, row);
                break;
            }
            case "daily-employee-wise":
            {
                var data = await GetDailyEmployeeWiseAsync(dailyFilter!);
                var ws = workbook.Worksheets.Add("Daily Employee Wise");
                WriteHeader(ws, new[] { "Employee ID", "Name", "Department", "Count", "Total Amount (₹)" });
                int row = 2;
                foreach (var r in data)
                {
                    ws.Cell(row, 1).Value = r.EmployeeIdNumber;
                    ws.Cell(row, 2).Value = r.EmployeeName;
                    ws.Cell(row, 3).Value = r.Department;
                    ws.Cell(row, 4).Value = r.TransactionCount;
                    ws.Cell(row, 5).Value = r.TotalAmount;
                    row++;
                }
                AddTotalsRow(ws, row, new[] { 4, 5 });
                FormatSheet(ws, row);
                break;
            }
            case "daily-summary":
            {
                var data = await GetDailySummaryAsync(dailyFilter!);
                var ws = workbook.Worksheets.Add("Daily Summary");
                WriteHeader(ws, new[] { "Time Schedule", "Slot", "Count", "Total Amount (₹)" });
                int row = 2;
                foreach (var r in data)
                {
                    ws.Cell(row, 1).Value = r.TimeScheduleName;
                    ws.Cell(row, 2).Value = r.TimeSlot;
                    ws.Cell(row, 3).Value = r.TransactionCount;
                    ws.Cell(row, 4).Value = r.TotalAmount;
                    row++;
                }
                AddTotalsRow(ws, row, new[] { 3, 4 });
                FormatSheet(ws, row);
                break;
            }
            case "employee-daily-summary":
            {
                var data = await GetEmployeeDailySummaryAsync(dailyFilter!);
                var ws = workbook.Worksheets.Add("Employee Daily Summary");
                WriteHeader(ws, new[] { "Employee ID", "Name", "Department", "Time Schedule", "Count", "Total Amount (₹)" });
                int row = 2;
                foreach (var r in data)
                {
                    ws.Cell(row, 1).Value = r.EmployeeIdNumber;
                    ws.Cell(row, 2).Value = r.EmployeeName;
                    ws.Cell(row, 3).Value = r.Department;
                    ws.Cell(row, 4).Value = r.TimeScheduleName;
                    ws.Cell(row, 5).Value = r.TransactionCount;
                    ws.Cell(row, 6).Value = r.TotalAmount;
                    row++;
                }
                AddTotalsRow(ws, row, new[] { 5, 6 });
                FormatSheet(ws, row);
                break;
            }
            case "monthly-food-wise":
            {
                var data = await GetMonthlyFoodWiseAsync(monthlyFilter!);
                var ws = workbook.Worksheets.Add("Monthly Food Wise");
                WriteHeader(ws, new[] { "Item", "Unit Price (₹)", "Count", "Total Amount (₹)" });
                int row = 2;
                foreach (var r in data)
                {
                    ws.Cell(row, 1).Value = r.ItemDescription;
                    ws.Cell(row, 2).Value = r.UnitPrice;
                    ws.Cell(row, 3).Value = r.TransactionCount;
                    ws.Cell(row, 4).Value = r.TotalAmount;
                    row++;
                }
                AddTotalsRow(ws, row, new[] { 3, 4 });
                FormatSheet(ws, row);
                break;
            }
            case "monthly-employee-wise":
            {
                var data = await GetMonthlyEmployeeWiseAsync(monthlyFilter!);
                var ws = workbook.Worksheets.Add("Monthly Employee Wise");
                WriteHeader(ws, new[] { "Employee ID", "Name", "Department", "Count", "Total Amount (₹)" });
                int row = 2;
                foreach (var r in data)
                {
                    ws.Cell(row, 1).Value = r.EmployeeIdNumber;
                    ws.Cell(row, 2).Value = r.EmployeeName;
                    ws.Cell(row, 3).Value = r.Department;
                    ws.Cell(row, 4).Value = r.TransactionCount;
                    ws.Cell(row, 5).Value = r.TotalAmount;
                    row++;
                }
                AddTotalsRow(ws, row, new[] { 4, 5 });
                FormatSheet(ws, row);
                break;
            }
            case "monthly-department-wise":
            {
                var data = await GetMonthlyDepartmentWiseAsync(monthlyFilter!);
                var ws = workbook.Worksheets.Add("Monthly Dept Wise");
                WriteHeader(ws, new[] { "Department", "CC Code", "Count", "Total Amount (₹)" });
                int row = 2;
                foreach (var r in data)
                {
                    ws.Cell(row, 1).Value = r.DepartmentName;
                    ws.Cell(row, 2).Value = r.CCCode;
                    ws.Cell(row, 3).Value = r.TransactionCount;
                    ws.Cell(row, 4).Value = r.TotalAmount;
                    row++;
                }
                AddTotalsRow(ws, row, new[] { 3, 4 });
                FormatSheet(ws, row);
                break;
            }
            case "employee-monthly-summary":
            {
                var data = await GetEmployeeMonthlySummaryAsync(monthlyFilter!);
                var ws = workbook.Worksheets.Add("Employee Monthly Summary");
                WriteHeader(ws, new[] { "Employee ID", "Name", "Department", "Time Schedule", "Count", "Total Amount (₹)" });
                int row = 2;
                foreach (var r in data)
                {
                    ws.Cell(row, 1).Value = r.EmployeeIdNumber;
                    ws.Cell(row, 2).Value = r.EmployeeName;
                    ws.Cell(row, 3).Value = r.Department;
                    ws.Cell(row, 4).Value = r.TimeScheduleName;
                    ws.Cell(row, 5).Value = r.TransactionCount;
                    ws.Cell(row, 6).Value = r.TotalAmount;
                    row++;
                }
                AddTotalsRow(ws, row, new[] { 5, 6 });
                FormatSheet(ws, row);
                break;
            }
            case "manual-punch":
            {
                var data = await GetManualPunchReportAsync(monthlyFilter!);
                var ws = workbook.Worksheets.Add("Manual Punch Report");
                WriteHeader(ws, new[] { "Employee ID", "Name", "Department", "Punch Time", "Schedule", "Item", "Price (₹)" });
                int row = 2;
                foreach (var r in data)
                {
                    ws.Cell(row, 1).Value = r.EmployeeIdNumber;
                    ws.Cell(row, 2).Value = r.EmployeeName;
                    ws.Cell(row, 3).Value = r.Department;
                    ws.Cell(row, 4).Value = r.PunchTime.ToString("yyyy-MM-dd HH:mm");
                    ws.Cell(row, 5).Value = r.TimeScheduleName;
                    ws.Cell(row, 6).Value = r.ItemDescription;
                    ws.Cell(row, 7).Value = r.Price;
                    row++;
                }
                AddTotalsRow(ws, row, new[] { 7 });
                FormatSheet(ws, row);
                break;
            }
            default:
                throw new ArgumentException($"Unknown report key: {reportKey}");
        }

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    // ── Excel helpers ────────────────────────────────────────────────────────

    private static void WriteHeader(IXLWorksheet ws, string[] headers)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
    }

    private static void AddTotalsRow(IXLWorksheet ws, int row, int[] numericColumns)
    {
        ws.Cell(row, 1).Value = "TOTAL";
        ws.Cell(row, 1).Style.Font.Bold = true;
        foreach (var col in numericColumns)
        {
            var totalsCell = ws.Cell(row, col);
            totalsCell.FormulaA1 = $"=SUM({ws.Cell(2, col).Address}:{ws.Cell(row - 1, col).Address})";
            totalsCell.Style.Font.Bold = true;
            totalsCell.Style.Fill.BackgroundColor = XLColor.LightYellow;
        }
    }

    private static void FormatSheet(IXLWorksheet ws, int lastRow)
    {
        ws.Columns().AdjustToContents();
        if (lastRow > 1)
            ws.Range(ws.Cell(1, 1), ws.Cell(lastRow, ws.LastColumnUsed().ColumnNumber()))
              .Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    }
}
