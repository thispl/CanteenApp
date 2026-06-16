using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// Employee entity representing staff members who can check in at the canteen.
/// Inherits from FullAuditedEntity to track creation, modification, deletion with user and timestamp.
/// </summary>
public class Employee : FullAuditedEntity<Guid>
{
    /// <summary>
    /// Unique business identifier/card number for the employee
    /// </summary>
    public virtual string EmployeeId { get; protected set; }

    /// <summary>
    /// Full name of the employee
    /// </summary>
    public virtual string FullName { get; protected set; }

    /// <summary>
    /// Department the employee belongs to (optional)
    /// </summary>
    public virtual string? Department { get; protected set; }

    protected Employee()
    {
        // Required by EF Core
    }

    public Employee(
        Guid id,
        string employeeId,
        string fullName,
        string? department = null)
    {
        Id = id;
        EmployeeId = employeeId;
        FullName = fullName;
        Department = department;
    }

    public virtual void SetFullName(string fullName)
    {
        FullName = fullName;
    }

    public virtual void SetDepartment(string? department)
    {
        Department = department;
    }
}
