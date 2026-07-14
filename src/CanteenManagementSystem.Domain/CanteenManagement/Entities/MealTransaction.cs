using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// A meal transaction generated either automatically from a ZKTeco punch
/// or entered manually through the Adjust/Manual Punch screens.
/// </summary>
public class MealTransaction : FullAuditedEntity<Guid>
{
    public virtual Guid EmployeeId { get; protected set; }
    public virtual Employee Employee { get; protected set; }

    public virtual Guid DeviceId { get; protected set; }
    public virtual Device Device { get; protected set; }

    public virtual Guid TimeScheduleId { get; protected set; }
    public virtual TimeSchedule TimeSchedule { get; protected set; }

    /// <summary>
    /// Denormalized item id copied from TimeSchedule at the time of creation.
    /// </summary>
    public virtual Guid ItemId { get; protected set; }

    public virtual Item Item { get; protected set; }

    /// <summary>
    /// Denormalized item price copied at the time of creation.
    /// </summary>
    public virtual decimal Price { get; protected set; }

    public virtual DateTime PunchTime { get; protected set; }

    public virtual MealTransactionSource Source { get; protected set; }

    protected MealTransaction()
    {
        // Required by EF Core
    }

    public MealTransaction(
        Guid id,
        Guid employeeId,
        Guid deviceId,
        Guid timeScheduleId,
        Guid itemId,
        decimal price,
        DateTime punchTime,
        MealTransactionSource source)
    {
        Id = id;
        EmployeeId = employeeId;
        DeviceId = deviceId;
        TimeScheduleId = timeScheduleId;
        ItemId = itemId;
        Price = price;
        PunchTime = punchTime;
        Source = source;
    }

    public virtual void SetPunchTime(DateTime punchTime)
    {
        PunchTime = punchTime;
    }

    public virtual void SetTimeSchedule(Guid timeScheduleId, Guid itemId, decimal price)
    {
        TimeScheduleId = timeScheduleId;
        ItemId = itemId;
        Price = price;
    }

    public virtual void SetEmployee(Guid employeeId)
    {
        EmployeeId = employeeId;
    }

    public virtual void SetDevice(Guid deviceId)
    {
        DeviceId = deviceId;
    }
}
