using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace CanteenManagementSystem.CanteenManagement.Repositories;

public interface IDeviceRepository : IRepository<Device, Guid>
{
    Task<Device?> FindByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default);

    Task<List<Device>> GetListAsync(
        string? filter = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
}
