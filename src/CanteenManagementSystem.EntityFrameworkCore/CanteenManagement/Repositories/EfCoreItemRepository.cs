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
/// EF Core implementation of IItemRepository
/// </summary>
public class EfCoreItemRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, Item, Guid>,
      IItemRepository
{
    public EfCoreItemRepository(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<List<Item>> GetListAsync(
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(i => i.Description.Contains(filter));
        }

        return await query
            .OrderBy(i => i.Description)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(i => i.Description.Contains(filter));
        }

        return await query.LongCountAsync(cancellationToken);
    }
}
