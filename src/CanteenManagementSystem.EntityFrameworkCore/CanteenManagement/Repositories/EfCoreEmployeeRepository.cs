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

/// <summary>
/// EF Core implementation of IEmployeeRepository
/// </summary>
public class EfCoreEmployeeRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, Employee, Guid>,
      IEmployeeRepository
{
    public EfCoreEmployeeRepository(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public override async Task<Employee> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        if (includeDetails)
        {
            query = query
                .Include(e => e.Department)
                .Include(e => e.Category)
                .Include(e => e.Designation);
        }
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Employee), id);
    }

    public virtual async Task<Employee?> FindByEmployeeIdAsync(
        string employeeId,
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(e => e.Department)
            .Include(e => e.Category)
            .Include(e => e.Designation)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId, cancellationToken);
    }

    public virtual async Task<bool> ExistsByEmployeeIdAsync(
        string employeeId,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .AnyAsync(e => e.EmployeeId == employeeId, cancellationToken);
    }

    public virtual async Task<List<Employee>> GetListAsync(
        string? filter = null,
        Guid? departmentId = null,
        Guid? categoryId = null,
        Guid? designationId = null,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        if (includeDetails)
        {
            query = query
                .Include(e => e.Department)
                .Include(e => e.Category)
                .Include(e => e.Designation);
        }

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(e =>
                e.EmployeeId.Contains(filter) ||
                e.FullName.Contains(filter));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == categoryId.Value);
        }

        if (designationId.HasValue)
        {
            query = query.Where(e => e.DesignationId == designationId.Value);
        }

        return await query
            .OrderBy(e => e.FullName)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filter = null,
        Guid? departmentId = null,
        Guid? categoryId = null,
        Guid? designationId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(e =>
                e.EmployeeId.Contains(filter) ||
                e.FullName.Contains(filter));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == categoryId.Value);
        }

        if (designationId.HasValue)
        {
            query = query.Where(e => e.DesignationId == designationId.Value);
        }

        return await query.LongCountAsync(cancellationToken);
    }
}
