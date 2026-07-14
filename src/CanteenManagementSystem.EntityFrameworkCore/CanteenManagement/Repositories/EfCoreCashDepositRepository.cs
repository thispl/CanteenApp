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

public class EfCoreCashDepositRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, CashDeposit, Guid>,
      ICashDepositRepository
{
    public EfCoreCashDepositRepository(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public override async Task<CashDeposit> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        if (includeDetails)
        {
            query = query.Include(d => d.Employee);
        }

        return await query.FirstOrDefaultAsync(d => d.Id == id, cancellationToken)
            ?? throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(CashDeposit), id);
    }

    public virtual async Task<long> GetCountAsync(
        Guid? employeeId = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = await BuildQueryAsync(employeeId, from, to, cancellationToken);
        return await query.LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<CashDeposit>> GetListAsync(
        Guid? employeeId = null,
        DateTime? from = null,
        DateTime? to = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = await BuildQueryAsync(employeeId, from, to, cancellationToken);

        if (includeDetails)
        {
            query = query.Include(d => d.Employee);
        }

        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "DepositDate desc" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    private async Task<IQueryable<CashDeposit>> BuildQueryAsync(
        Guid? employeeId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken)
    {
        var query = await GetQueryableAsync();

        if (employeeId.HasValue)
        {
            query = query.Where(d => d.EmployeeId == employeeId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(d => d.DepositDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(d => d.DepositDate <= to.Value);
        }

        return query;
    }
}
