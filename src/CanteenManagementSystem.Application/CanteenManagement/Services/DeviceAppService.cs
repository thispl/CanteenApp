using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service for Device management
/// </summary>
[Authorize]
public class DeviceAppService : ApplicationService, IDeviceAppService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IGuidGenerator _guidGenerator;

    public DeviceAppService(
        IDeviceRepository deviceRepository,
        IGuidGenerator guidGenerator)
    {
        _deviceRepository = deviceRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<DeviceDto?> GetAsync(Guid id)
    {
        var device = await _deviceRepository.GetAsync(id);
        return ObjectMapper.Map<Device, DeviceDto>(device);
    }

    public virtual async Task<DeviceDto?> GetByDeviceIdAsync(string deviceId)
    {
        var device = await _deviceRepository.FindByDeviceIdAsync(deviceId);
        if (device == null)
        {
            return null;
        }
        return ObjectMapper.Map<Device, DeviceDto>(device);
    }

    public virtual async Task<PagedResultDto<DeviceDto>> GetListAsync(DeviceListFilterDto input)
    {
        var count = await _deviceRepository.GetCountAsync(input.Filter);
        var devices = await _deviceRepository.GetListAsync(
            input.Filter,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            CancellationToken.None);

        return new PagedResultDto<DeviceDto>(
            count,
            ObjectMapper.Map<List<Device>, List<DeviceDto>>(devices));
    }

    public virtual async Task<DeviceDto> CreateAsync(CreateDeviceDto input)
    {
        if (await _deviceRepository.ExistsByDeviceIdAsync(input.DeviceId))
        {
            throw new UserFriendlyException($"Device with ID '{input.DeviceId}' already exists.");
        }

        var device = new Device(
            _guidGenerator.Create(),
            input.DeviceId,
            input.Name,
            input.IpAddress,
            input.Port,
            input.Status,
            input.Location,
            input.Model,
            input.SerialNumber);

        await _deviceRepository.InsertAsync(device);

        return ObjectMapper.Map<Device, DeviceDto>(device);
    }

    public virtual async Task<DeviceDto> UpdateAsync(Guid id, UpdateDeviceDto input)
    {
        var device = await _deviceRepository.GetAsync(id);

        if (input.DeviceId != device.DeviceId &&
            await _deviceRepository.ExistsByDeviceIdAsync(input.DeviceId))
        {
            throw new UserFriendlyException($"Device with ID '{input.DeviceId}' already exists.");
        }

        device.SetDeviceId(input.DeviceId);
        device.SetName(input.Name);
        device.SetIpAddress(input.IpAddress);
        device.SetPort(input.Port);
        device.SetStatus(input.Status);
        device.SetLocation(input.Location);
        device.SetModel(input.Model);
        device.SetSerialNumber(input.SerialNumber);

        await _deviceRepository.UpdateAsync(device);

        return ObjectMapper.Map<Device, DeviceDto>(device);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _deviceRepository.DeleteAsync(id);
    }

    public virtual async Task<bool> ExistsByDeviceIdAsync(string deviceId)
    {
        return await _deviceRepository.ExistsByDeviceIdAsync(deviceId);
    }
}
