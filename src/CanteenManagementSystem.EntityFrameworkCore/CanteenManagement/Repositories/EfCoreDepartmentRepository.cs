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
/// EF Core implementation of IDepartmentRepository
/// </summary>
public class EfCoreDepartmentRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, Department, Guid>,
      IDepartmentRepository
{
    public EfCoreDepartmentRepository(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<Department?> FindByCCCodeAsync(
        string ccCode,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(d => d.CCCode == ccCode, cancellationToken);
    }

    public virtual async Task<bool> ExistsByCCCodeAsync(
        string ccCode,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .AnyAsync(d => d.CCCode == ccCode, cancellationToken);
    }

    public virtual async Task<List<Department>> GetListAsync(
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(d =>
                d.Name.Contains(filter) ||
                (d.CCCode != null && d.CCCode.Contains(filter)));
        }

        return await query
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(d =>
                d.Name.Contains(filter) ||
                (d.CCCode != null && d.CCCode.Contains(filter)));
        }

        return await query.LongCountAsync(cancellationToken);
    }
}
