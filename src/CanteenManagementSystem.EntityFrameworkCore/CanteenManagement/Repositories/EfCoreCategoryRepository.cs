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
/// EF Core implementation of ICategoryRepository
/// </summary>
public class EfCoreCategoryRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, Category, Guid>,
      ICategoryRepository
{
    public EfCoreCategoryRepository(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<Category?> FindByCodeAsync(
        string categoryCode,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(c => c.CategoryCode == categoryCode, cancellationToken);
    }

    public virtual async Task<bool> ExistsByCodeAsync(
        string categoryCode,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .AnyAsync(c => c.CategoryCode == categoryCode, cancellationToken);
    }

    public virtual async Task<List<Category>> GetListAsync(
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(c =>
                c.CategoryName.Contains(filter) ||
                (c.CategoryCode != null && c.CategoryCode.Contains(filter)));
        }

        return await query
            .OrderBy(c => c.CategoryName)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(c =>
                c.CategoryName.Contains(filter) ||
                (c.CategoryCode != null && c.CategoryCode.Contains(filter)));
        }

        return await query.LongCountAsync(cancellationToken);
    }
}
