using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// SyncState entity tracks synchronization progress markers.
/// Inherits from CreationAuditedEntity since updates modify the value directly.
/// </summary>
public class SyncState : CreationAuditedEntity<Guid>
{
    /// <summary>
    /// Unique dictionary key for the sync state (e.g., "Last_Zk_Transaction_Id")
    /// </summary>
    public virtual string Key { get; protected set; }

    /// <summary>
    /// Last processed value marker (e.g., highest transaction ID processed)
    /// </summary>
    public virtual int LastProcessedValue { get; protected set; }

    protected SyncState()
    {
        // Required by EF Core
    }

    public SyncState(
        Guid id,
        string key,
        int lastProcessedValue = 0)
    {
        Id = id;
        Key = key;
        LastProcessedValue = lastProcessedValue;
    }

    public virtual void UpdateLastProcessedValue(int value)
    {
        LastProcessedValue = value;
    }
}
