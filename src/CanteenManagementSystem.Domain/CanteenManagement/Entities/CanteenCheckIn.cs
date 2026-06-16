using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// Canteen check-in record representing a punch at the canteen turnstile.
/// Inherits from FullAuditedEntity to track creation, modification, deletion with user and timestamp.
/// </summary>
public class CanteenCheckIn : FullAuditedEntity<Guid>
{
    /// <summary>
    /// Employee ID (business identifier) who performed the check-in
    /// </summary>
    public virtual string EmployeeId { get; protected set; }

    /// <summary>
    /// Device ID (turnstile identifier) that recorded the check-in
    /// </summary>
    public virtual string DeviceId { get; protected set; }

    /// <summary>
    /// Official time when the check-in occurred
    /// </summary>
    public virtual DateTime CheckInTime { get; protected set; }

    protected CanteenCheckIn()
    {
        // Required by EF Core
    }

    public CanteenCheckIn(
        Guid id,
        string employeeId,
        string deviceId,
        DateTime checkInTime)
    {
        Id = id;
        EmployeeId = employeeId;
        DeviceId = deviceId;
        CheckInTime = checkInTime;
    }
}
