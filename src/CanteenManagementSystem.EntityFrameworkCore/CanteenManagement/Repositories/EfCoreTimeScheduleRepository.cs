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

public class EfCoreTimeScheduleRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, TimeSchedule, Guid>,
      ITimeScheduleRepository
{
    public EfCoreTimeScheduleRepository(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<TimeSchedule?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
    }

    public virtual async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(t => t.Code == code, cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .WhereIf(!filter.IsNullOrWhiteSpace(), t =>
                t.Name.Contains(filter!) ||
                (t.Code != null && t.Code.Contains(filter!)))
            .LongCountAsync(cancellationToken);
    }

    public override async Task<TimeSchedule> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        if (includeDetails)
        {
            query = query.Include(t => t.Item);
        }
        return await query.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(TimeSchedule), id);
    }

    public virtual async Task<List<TimeSchedule>> GetListAsync(
        string? filter = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = query
            .WhereIf(!filter.IsNullOrWhiteSpace(), t =>
                t.Name.Contains(filter!) ||
                (t.Code != null && t.Code.Contains(filter!)))
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "StartTime asc" : sorting)
            .PageBy(skipCount, maxResultCount);

        if (includeDetails)
        {
            query = query.Include(t => t.Item);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
