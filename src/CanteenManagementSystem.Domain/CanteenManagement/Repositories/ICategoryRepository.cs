using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

/// <summary>
/// Repository interface for Category entity
/// </summary>
public interface ICategoryRepository : IRepository<Category, Guid>
{
    /// <summary>
    /// Find category by its unique code
    /// </summary>
    Task<Category?> FindByCodeAsync(
        string categoryCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if category exists by code
    /// </summary>
    Task<bool> ExistsByCodeAsync(
        string categoryCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list of categories with optional filtering
    /// </summary>
    Task<List<Category>> GetListAsync(
        string? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of categories with optional filtering
    /// </summary>
    Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default);
}
