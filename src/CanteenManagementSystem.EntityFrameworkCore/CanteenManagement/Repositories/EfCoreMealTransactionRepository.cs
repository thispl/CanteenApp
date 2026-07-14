using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using CanteenManagementSystem.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

public class EfCoreMealTransactionRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, MealTransaction, Guid>,
      IMealTransactionRepository
{
    public EfCoreMealTransactionRepository(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public override async Task<MealTransaction> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        if (includeDetails)
        {
            query = query
                .Include(t => t.Employee)
                .Include(t => t.Device)
                .Include(t => t.TimeSchedule)
                .Include(t => t.Item);
        }
        return await query.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(MealTransaction), id);
    }

    public virtual async Task<bool> ExistsAsync(
        Guid employeeId,
        DateTime punchTime,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .AnyAsync(t => t.EmployeeId == employeeId && t.PunchTime == punchTime, cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        Guid? employeeId = null,
        Guid? timeScheduleId = null,
        DateTime? from = null,
        DateTime? to = null,
        MealTransactionSource? source = null,
        CancellationToken cancellationToken = default)
    {
        var query = await BuildQueryAsync(employeeId, timeScheduleId, from, to, source, cancellationToken);
        return await query.LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<MealTransaction>> GetListAsync(
        Guid? employeeId = null,
        Guid? timeScheduleId = null,
        DateTime? from = null,
        DateTime? to = null,
        MealTransactionSource? source = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = await BuildQueryAsync(employeeId, timeScheduleId, from, to, source, cancellationToken);

        if (includeDetails)
        {
            query = query
                .Include(t => t.Employee)
                .Include(t => t.Device)
                .Include(t => t.TimeSchedule)
                .Include(t => t.Item);
        }

        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "PunchTime desc" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    private async Task<IQueryable<MealTransaction>> BuildQueryAsync(
        Guid? employeeId,
        Guid? timeScheduleId,
        DateTime? from,
        DateTime? to,
        MealTransactionSource? source,
        CancellationToken cancellationToken)
    {
        var query = await GetQueryableAsync();

        if (employeeId.HasValue)
        {
            query = query.Where(t => t.EmployeeId == employeeId.Value);
        }

        if (timeScheduleId.HasValue)
        {
            query = query.Where(t => t.TimeScheduleId == timeScheduleId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(t => t.PunchTime >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(t => t.PunchTime <= to.Value);
        }

        if (source.HasValue)
        {
            query = query.Where(t => t.Source == source.Value);
        }

        return query;
    }
}
