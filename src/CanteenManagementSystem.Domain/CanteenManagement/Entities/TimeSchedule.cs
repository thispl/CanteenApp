using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// Time schedule master representing canteen meal slots.
/// </summary>
public class TimeSchedule : FullAuditedEntity<Guid>
{
    /// <summary>
    /// Name of the schedule (e.g., Breakfast, Lunch, Dinner)
    /// </summary>
    public virtual string Name { get; protected set; }

    /// <summary>
    /// Optional short code
    /// </summary>
    public virtual string? Code { get; protected set; }

    /// <summary>
    /// Start time of the schedule
    /// </summary>
    public virtual TimeOnly StartTime { get; protected set; }

    /// <summary>
    /// End time of the schedule
    /// </summary>
    public virtual TimeOnly EndTime { get; protected set; }

    /// <summary>
    /// Linked canteen item for this schedule
    /// </summary>
    public virtual Guid? ItemId { get; protected set; }

    public virtual Item? Item { get; protected set; }

    protected TimeSchedule()
    {
        // Required by EF Core
    }

    public TimeSchedule(
        Guid id,
        string name,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? itemId,
        string? code = null)
    {
        Id = id;
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        ItemId = itemId;
        Code = code;
    }

    public virtual void SetName(string name)
    {
        Name = name;
    }

    public virtual void SetCode(string? code)
    {
        Code = code;
    }

    public virtual void SetStartTime(TimeOnly startTime)
    {
        StartTime = startTime;
    }

    public virtual void SetEndTime(TimeOnly endTime)
    {
        EndTime = endTime;
    }

    public virtual void SetItem(Guid? itemId)
    {
        ItemId = itemId;
    }
}
