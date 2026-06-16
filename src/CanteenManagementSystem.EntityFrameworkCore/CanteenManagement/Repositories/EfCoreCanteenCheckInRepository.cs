using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using CanteenManagementSystem.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

/// <summary>
/// EF Core implementation of ICanteenCheckInRepository
/// </summary>
public class EfCoreCanteenCheckInRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, CanteenCheckIn, Guid>,
      ICanteenCheckInRepository
{
    public EfCoreCanteenCheckInRepository(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<bool> ExistsAsync(
        string employeeId,
        string deviceId,
        DateTime checkInTime,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .AnyAsync(c =>
                c.EmployeeId == employeeId &&
                c.DeviceId == deviceId &&
                c.CheckInTime == checkInTime,
                cancellationToken);
    }

    public virtual async Task<List<CanteenCheckIn>> GetListChronologicalAsync(
        int maxResultCount = 50,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .OrderByDescending(c => c.CheckInTime)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<List<CanteenCheckInWithEmployee>> GetListWithEmployeeAsync(
        int maxResultCount = 50,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var query = from checkIn in dbContext.Set<CanteenCheckIn>()
                    join employee in dbContext.Set<Employee>()
                        on checkIn.EmployeeId equals employee.EmployeeId into employeeJoin
                    from employee in employeeJoin.DefaultIfEmpty()
                    orderby checkIn.CheckInTime descending
                    select new CanteenCheckInWithEmployee
                    {
                        Id = checkIn.Id,
                        EmployeeId = checkIn.EmployeeId,
                        FullName = employee != null ? employee.FullName : "Unknown",
                        Department = employee != null ? employee.Department : null,
                        DeviceId = checkIn.DeviceId,
                        CheckInTime = checkIn.CheckInTime
                    };

        return await query
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? employeeId = null,
        string? deviceId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(employeeId))
        {
            query = query.Where(c => c.EmployeeId == employeeId);
        }

        if (!string.IsNullOrWhiteSpace(deviceId))
        {
            query = query.Where(c => c.DeviceId == deviceId);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(c => c.CheckInTime >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(c => c.CheckInTime <= toDate.Value);
        }

        return await query.LongCountAsync(cancellationToken);
    }
}
