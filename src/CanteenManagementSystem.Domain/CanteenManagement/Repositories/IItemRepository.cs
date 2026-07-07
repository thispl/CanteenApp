using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

/// <summary>
/// Repository interface for Item entity
/// </summary>
public interface IItemRepository : IRepository<Item, Guid>
{
    /// <summary>
    /// Get list of items with optional filtering
    /// </summary>
    Task<List<Item>> GetListAsync(
        string? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of items with optional filtering
    /// </summary>
    Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default);
}
