using System;

namespace CanteenManagementSystem.EntityFrameworkCore.ZkTecoIntegration;

/// <summary>
/// External Transaction entity from ZKTeco iclock_transaction table.
/// Read-only entity mapped to external database.
/// </summary>
public class ExternalTransaction
{
    /// <summary>
    /// Primary key - transaction ID (INT)
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// PIN matches EnrollNumber/EmployeeId
    /// </summary>
    public virtual string? Pin { get; set; }

    /// <summary>
    /// Device ID that recorded the punch
    /// </summary>
    public virtual string? AuthDeviceId { get; set; }

    /// <summary>
    /// Timestamp of the punch
    /// </summary>
    public virtual DateTime? AuthTime { get; set; }
}
