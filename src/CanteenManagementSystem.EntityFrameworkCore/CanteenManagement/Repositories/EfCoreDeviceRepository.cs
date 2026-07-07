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

public class EfCoreDeviceRepository
    : EfCoreRepository<CanteenManagementSystemDbContext, Device, Guid>,
      IDeviceRepository
{
    public EfCoreDeviceRepository(IDbContextProvider<CanteenManagementSystemDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<Device?> FindByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(d => d.DeviceId == deviceId, cancellationToken);
    }

    public virtual async Task<bool> ExistsByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(d => d.DeviceId == deviceId, cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .WhereIf(!filter.IsNullOrWhiteSpace(), d =>
                d.Name.Contains(filter!) ||
                d.DeviceId.Contains(filter!) ||
                (d.IpAddress != null && d.IpAddress.Contains(filter!)) ||
                (d.Location != null && d.Location.Contains(filter!)) ||
                (d.Model != null && d.Model.Contains(filter!)) ||
                (d.SerialNumber != null && d.SerialNumber.Contains(filter!)))
            .LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<Device>> GetListAsync(
        string? filter = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .WhereIf(!filter.IsNullOrWhiteSpace(), d =>
                d.Name.Contains(filter!) ||
                d.DeviceId.Contains(filter!) ||
                (d.IpAddress != null && d.IpAddress.Contains(filter!)) ||
                (d.Location != null && d.Location.Contains(filter!)) ||
                (d.Model != null && d.Model.Contains(filter!)) ||
                (d.SerialNumber != null && d.SerialNumber.Contains(filter!)))
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "Name asc" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }
}
