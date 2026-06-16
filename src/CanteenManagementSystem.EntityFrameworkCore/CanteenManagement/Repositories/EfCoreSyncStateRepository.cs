using System;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using CanteenManagementSystem.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

/// <summary>
/// EF Core implementation of ISyncStateRepository
/// </summary>
public class EfCoreSyncStateRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, SyncState, Guid>,
      ISyncStateRepository
{
    private readonly IGuidGenerator _guidGenerator;

    public EfCoreSyncStateRepository(
        IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider,
        IGuidGenerator guidGenerator)
        : base(dbContextProvider)
    {
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<SyncState?> GetByKeyAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }

    public virtual async Task<SyncState> GetOrCreateAsync(
        string key,
        int defaultValue = 0,
        CancellationToken cancellationToken = default)
    {
        var syncState = await GetByKeyAsync(key, cancellationToken);

        if (syncState == null)
        {
            syncState = new SyncState(
                _guidGenerator.Create(),
                key,
                defaultValue);

            await InsertAsync(syncState, autoSave: true, cancellationToken);
        }

        return syncState;
    }
}
