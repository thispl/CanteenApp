using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// A cash deposit recorded for an employee to offset meal transactions.
/// </summary>
public class CashDeposit : FullAuditedEntity<Guid>
{
    public virtual Guid EmployeeId { get; protected set; }
    public virtual Employee Employee { get; protected set; }

    public virtual decimal Amount { get; protected set; }

    public virtual DateTime DepositDate { get; protected set; }

    public virtual string? Notes { get; protected set; }

    protected CashDeposit()
    {
        // Required by EF Core
    }

    public CashDeposit(
        Guid id,
        Guid employeeId,
        decimal amount,
        DateTime depositDate,
        string? notes = null)
    {
        Id = id;
        EmployeeId = employeeId;
        Amount = amount;
        DepositDate = depositDate;
        Notes = notes;
    }

    public virtual void Update(
        Guid employeeId,
        decimal amount,
        DateTime depositDate,
        string? notes = null)
    {
        EmployeeId = employeeId;
        Amount = amount;
        DepositDate = depositDate;
        Notes = notes;
    }
}
