using System;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

/// <summary>
/// Repository interface for SyncState entity
/// </summary>
public interface ISyncStateRepository : IRepository<SyncState, Guid>
{
    /// <summary>
    /// Get sync state by key
    /// Returns null if not found
    /// </summary>
    Task<SyncState?> GetByKeyAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get or create sync state with default value
    /// </summary>
    Task<SyncState> GetOrCreateAsync(
        string key,
        int defaultValue = 0,
        CancellationToken cancellationToken = default);
}
