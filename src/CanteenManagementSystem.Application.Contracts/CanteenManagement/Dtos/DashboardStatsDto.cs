using System;
using System.Collections.Generic;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

/// <summary>
/// Dashboard statistics DTO
/// </summary>
public class DashboardStatsDto
{
    /// <summary>
    /// Total number of employees
    /// </summary>
    public long TotalEmployees { get; set; }

    /// <summary>
    /// Number of check-ins today
    /// </summary>
    public long TodayCheckIns { get; set; }

    /// <summary>
    /// Number of unique employees checked in today
    /// </summary>
    public long TodayUniqueCheckIns { get; set; }

    /// <summary>
    /// Number of check-ins this week
    /// </summary>
    public long WeekCheckIns { get; set; }

    /// <summary>
    /// Number of check-ins this month
    /// </summary>
    public long MonthCheckIns { get; set; }

    /// <summary>
    /// Latest check-ins (last 10)
    /// </summary>
    public List<LatestCheckInDto> LatestCheckIns { get; set; } = new();

    /// <summary>
    /// Top employees by check-in count today
    /// </summary>
    public List<TopEmployeeDto> TopEmployeesToday { get; set; } = new();

    /// <summary>
    /// Hourly check-in distribution for today
    /// </summary>
    public List<HourlyStatsDto> HourlyDistribution { get; set; } = new();
}

/// <summary>
/// Latest check-in DTO for dashboard
/// </summary>
public class LatestCheckInDto
{
    public string EmployeeId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Department { get; set; }
    public DateTime CheckInTime { get; set; }
    public string DeviceId { get; set; } = null!;
}

/// <summary>
/// Top employee DTO for dashboard
/// </summary>
public class TopEmployeeDto
{
    public string EmployeeId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Department { get; set; }
    public long CheckInCount { get; set; }
}

/// <summary>
/// Hourly statistics DTO
/// </summary>
public class HourlyStatsDto
{
    public int Hour { get; set; }
    public long Count { get; set; }
}
