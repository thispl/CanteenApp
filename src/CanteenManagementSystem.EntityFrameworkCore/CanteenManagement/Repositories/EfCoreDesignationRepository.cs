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

public class EfCoreDesignationRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, Designation, Guid>,
      IDesignationRepository
{
    public EfCoreDesignationRepository(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<Designation?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(d => d.Code == code, cancellationToken);
    }

    public virtual async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(d => d.Code == code, cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .WhereIf(!filter.IsNullOrWhiteSpace(), d =>
                d.Title.Contains(filter!) ||
                (d.Code != null && d.Code.Contains(filter!)) ||
                (d.Description != null && d.Description.Contains(filter!)))
            .LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<Designation>> GetListAsync(
        string? filter = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .WhereIf(!filter.IsNullOrWhiteSpace(), d =>
                d.Title.Contains(filter!) ||
                (d.Code != null && d.Code.Contains(filter!)) ||
                (d.Description != null && d.Description.Contains(filter!)))
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "Title asc" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }
}
