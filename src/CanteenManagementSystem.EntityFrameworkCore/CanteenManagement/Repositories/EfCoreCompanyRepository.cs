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

public class EfCoreCompanyRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, Company, Guid>,
      ICompanyRepository
{
    public EfCoreCompanyRepository(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<Company?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public virtual async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(c => c.Code == code, cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .WhereIf(!filter.IsNullOrWhiteSpace(), c =>
                c.Name.Contains(filter!) ||
                (c.Code != null && c.Code.Contains(filter!)) ||
                (c.Address != null && c.Address.Contains(filter!)) ||
                (c.Email != null && c.Email.Contains(filter!)))
            .LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<Company>> GetListAsync(
        string? filter = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .WhereIf(!filter.IsNullOrWhiteSpace(), c =>
                c.Name.Contains(filter!) ||
                (c.Code != null && c.Code.Contains(filter!)) ||
                (c.Address != null && c.Address.Contains(filter!)) ||
                (c.Email != null && c.Email.Contains(filter!)))
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "Name asc" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }
}
