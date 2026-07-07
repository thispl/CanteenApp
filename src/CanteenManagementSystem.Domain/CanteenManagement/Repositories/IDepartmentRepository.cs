using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

/// <summary>
/// Repository interface for Department entity
/// </summary>
public interface IDepartmentRepository : IRepository<Department, Guid>
{
    /// <summary>
    /// Find department by its cost-center code
    /// </summary>
    Task<Department?> FindByCCCodeAsync(
        string ccCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if department exists by cost-center code
    /// </summary>
    Task<bool> ExistsByCCCodeAsync(
        string ccCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list of departments with optional filtering
    /// </summary>
    Task<List<Department>> GetListAsync(
        string? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of departments with optional filtering
    /// </summary>
    Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default);
}
