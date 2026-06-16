using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service for dashboard statistics
/// </summary>
[Authorize]
public class DashboardAppService : ApplicationService, IDashboardAppService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICanteenCheckInRepository _checkInRepository;

    public DashboardAppService(
        IEmployeeRepository employeeRepository,
        ICanteenCheckInRepository checkInRepository)
    {
        _employeeRepository = employeeRepository;
        _checkInRepository = checkInRepository;
    }

    public virtual async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var now = Clock.Now;
        var today = now.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var tomorrow = today.AddDays(1);
        var weekEnd = weekStart.AddDays(7);
        var monthEnd = monthStart.AddMonths(1);

        // Get total employees
        var totalEmployees = await _employeeRepository.GetCountAsync();

        // Get today's check-ins count
        var todayCheckIns = await _checkInRepository.GetCountAsync(
            fromDate: today,
            toDate: tomorrow);

        // Get this week's check-ins
        var weekCheckIns = await _checkInRepository.GetCountAsync(
            fromDate: weekStart,
            toDate: weekEnd);

        // Get this month's check-ins
        var monthCheckIns = await _checkInRepository.GetCountAsync(
            fromDate: monthStart,
            toDate: monthEnd);

        // Get latest check-ins with employee info
        var latestCheckIns = await _checkInRepository.GetListWithEmployeeAsync(10);

        // Calculate unique check-ins today
        var todayUniqueCheckIns = latestCheckIns
            .Where(c => c.CheckInTime.Date == today)
            .Select(c => c.EmployeeId)
            .Distinct()
            .Count();

        // Get hourly distribution for today
        var todayCheckInsList = latestCheckIns
            .Where(c => c.CheckInTime.Date == today)
            .ToList();

        var hourlyDistribution = Enumerable.Range(6, 18) // 6 AM to 11 PM
            .Select(hour => new HourlyStatsDto
            {
                Hour = hour,
                Count = todayCheckInsList.Count(c => c.CheckInTime.Hour == hour)
            })
            .ToList();

        // Get top employees by check-in count today
        var topEmployeesToday = todayCheckInsList
            .GroupBy(c => new { c.EmployeeId, c.FullName, c.Department })
            .Select(g => new TopEmployeeDto
            {
                EmployeeId = g.Key.EmployeeId,
                FullName = g.Key.FullName,
                Department = g.Key.Department,
                CheckInCount = g.Count()
            })
            .OrderByDescending(e => e.CheckInCount)
            .Take(5)
            .ToList();

        return new DashboardStatsDto
        {
            TotalEmployees = totalEmployees,
            TodayCheckIns = todayCheckIns,
            TodayUniqueCheckIns = todayUniqueCheckIns,
            WeekCheckIns = weekCheckIns,
            MonthCheckIns = monthCheckIns,
            LatestCheckIns = latestCheckIns
                .Select(c => new LatestCheckInDto
                {
                    EmployeeId = c.EmployeeId,
                    FullName = c.FullName,
                    Department = c.Department,
                    CheckInTime = c.CheckInTime,
                    DeviceId = c.DeviceId
                })
                .ToList(),
            TopEmployeesToday = topEmployeesToday,
            HourlyDistribution = hourlyDistribution
        };
    }
}
