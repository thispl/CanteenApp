using System;

namespace CanteenManagementSystem.CanteenManagement.Reports;

// ─── Filter DTOs ────────────────────────────────────────────────────────────

/// <summary>Filter for daily (single-date) reports.</summary>
public class DailyReportFilterDto
{
    public DateTime Date { get; set; } = DateTime.Today;
    public Guid? EmployeeId { get; set; }
}

/// <summary>Filter for monthly / date-range reports.</summary>
public class MonthlyReportFilterDto
{
    public DateTime From { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    public DateTime To { get; set; } = DateTime.Today;
    public Guid? EmployeeId { get; set; }
    public Guid? DepartmentId { get; set; }
}

// ─── Result DTOs ─────────────────────────────────────────────────────────────

/// <summary>Daily / Monthly Count Report – Food Wise row.</summary>
public class FoodWiseReportRowDto
{
    public string ItemDescription { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>Daily / Monthly Count Report – Employee Wise row.</summary>
public class EmployeeWiseReportRowDto
{
    public string EmployeeIdNumber { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>Daily Summary Report – Time Schedule Wise row.</summary>
public class DailySummaryReportRowDto
{
    public string TimeScheduleName { get; set; } = string.Empty;
    public string TimeSlot { get; set; } = string.Empty;   // e.g. "08:00 – 09:00"
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>Monthly Count Report – Department Wise row.</summary>
public class DepartmentWiseReportRowDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public string CCCode { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>Employee Wise Monthly Summary – one row per employee per time-schedule.</summary>
public class EmployeeMonthlyDetailRowDto
{
    public string EmployeeIdNumber { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string TimeScheduleName { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>Employee Manual Punch Report row.</summary>
public class ManualPunchReportRowDto
{
    public string EmployeeIdNumber { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime PunchTime { get; set; }
    public string TimeScheduleName { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

/// <summary>Employee Wise Daily Summary – one row per employee per time-schedule for a date.</summary>
public class EmployeeDailySummaryRowDto
{
    public string EmployeeIdNumber { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string TimeScheduleName { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}
