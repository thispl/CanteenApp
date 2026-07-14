using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

/// <summary>
/// Repository interface for Employee entity
/// </summary>
public interface IEmployeeRepository : IRepository<Employee, Guid>
{
    /// <summary>
    /// Find employee by their business EmployeeId
    /// </summary>
    Task<Employee?> FindByEmployeeIdAsync(
        string employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if employee exists by EmployeeId
    /// </summary>
    Task<bool> ExistsByEmployeeIdAsync(
        string employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list of employees with optional filtering
    /// </summary>
    Task<List<Employee>> GetListAsync(
        string? filter = null,
        Guid? departmentId = null,
        Guid? categoryId = null,
        Guid? designationId = null,
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of employees with optional filtering
    /// </summary>
    Task<long> GetCountAsync(
        string? filter = null,
        Guid? departmentId = null,
        Guid? categoryId = null,
        Guid? designationId = null,
        CancellationToken cancellationToken = default);
}
