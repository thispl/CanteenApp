using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

public interface IDeviceAppService : IApplicationService
{
    Task<DeviceDto?> GetAsync(Guid id);

    Task<DeviceDto?> GetByDeviceIdAsync(string deviceId);

    Task<PagedResultDto<DeviceDto>> GetListAsync(DeviceListFilterDto input);

    Task<DeviceDto> CreateAsync(CreateDeviceDto input);

    Task<DeviceDto> UpdateAsync(Guid id, UpdateDeviceDto input);

    Task DeleteAsync(Guid id);

    Task<bool> ExistsByDeviceIdAsync(string deviceId);
}
