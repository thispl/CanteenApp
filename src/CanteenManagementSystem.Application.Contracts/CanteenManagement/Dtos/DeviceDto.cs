using System;
using Volo.Abp.Application.Dtos;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

/// <summary>
/// DTO for Device entity
/// </summary>
public class DeviceDto : AuditedEntityDto<Guid>
{
    public string DeviceId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? IpAddress { get; set; }
    public int? Port { get; set; }
    public DeviceStatus Status { get; set; }
    public string? Location { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
}

/// <summary>
/// DTO for creating a new device
/// </summary>
public class CreateDeviceDto
{
    public string DeviceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public int? Port { get; set; }
    public DeviceStatus Status { get; set; } = DeviceStatus.Active;
    public string? Location { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
}

/// <summary>
/// DTO for updating a device
/// </summary>
public class UpdateDeviceDto
{
    public string DeviceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public int? Port { get; set; }
    public DeviceStatus Status { get; set; } = DeviceStatus.Active;
    public string? Location { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
}

/// <summary>
/// DTO for filtering devices
/// </summary>
public class DeviceListFilterDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
