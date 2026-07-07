using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// Device master representing biometric / access control devices.
/// </summary>
public class Device : FullAuditedEntity<Guid>
{
    /// <summary>
    /// Unique device identifier from the hardware
    /// </summary>
    public virtual string DeviceId { get; protected set; }

    /// <summary>
    /// Friendly device name
    /// </summary>
    public virtual string Name { get; protected set; }

    /// <summary>
    /// Device IP address
    /// </summary>
    public virtual string? IpAddress { get; protected set; }

    /// <summary>
    /// Device port number
    /// </summary>
    public virtual int? Port { get; protected set; }

    /// <summary>
    /// Current device status
    /// </summary>
    public virtual DeviceStatus Status { get; protected set; }

    /// <summary>
    /// Device location description
    /// </summary>
    public virtual string? Location { get; protected set; }

    /// <summary>
    /// Device model
    /// </summary>
    public virtual string? Model { get; protected set; }

    /// <summary>
    /// Device serial number
    /// </summary>
    public virtual string? SerialNumber { get; protected set; }

    protected Device()
    {
        // Required by EF Core
    }

    public Device(
        Guid id,
        string deviceId,
        string name,
        string? ipAddress = null,
        int? port = null,
        DeviceStatus status = DeviceStatus.Active,
        string? location = null,
        string? model = null,
        string? serialNumber = null)
    {
        Id = id;
        DeviceId = deviceId;
        Name = name;
        IpAddress = ipAddress;
        Port = port;
        Status = status;
        Location = location;
        Model = model;
        SerialNumber = serialNumber;
    }

    public virtual void SetDeviceId(string deviceId)
    {
        DeviceId = deviceId;
    }

    public virtual void SetName(string name)
    {
        Name = name;
    }

    public virtual void SetIpAddress(string? ipAddress)
    {
        IpAddress = ipAddress;
    }

    public virtual void SetPort(int? port)
    {
        Port = port;
    }

    public virtual void SetStatus(DeviceStatus status)
    {
        Status = status;
    }

    public virtual void SetLocation(string? location)
    {
        Location = location;
    }

    public virtual void SetModel(string? model)
    {
        Model = model;
    }

    public virtual void SetSerialNumber(string? serialNumber)
    {
        SerialNumber = serialNumber;
    }
}
