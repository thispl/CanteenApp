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
    public virtual Guid? DepartmentId { get; protected set; }
    public virtual Department? Department { get; protected set; }

    /// <summary>
    /// Category the employee belongs to (optional)
    /// </summary>
    public virtual Guid? CategoryId { get; protected set; }
    public virtual Category? Category { get; protected set; }

    /// <summary>
    /// Designation of the employee (optional)
    /// </summary>
    public virtual Guid? DesignationId { get; protected set; }
    public virtual Designation? Designation { get; protected set; }

    protected Employee()
    {
        // Required by EF Core
    }

    public Employee(
        Guid id,
        string employeeId,
        string fullName,
        Guid? departmentId = null,
        Guid? categoryId = null,
        Guid? designationId = null)
    {
        Id = id;
        EmployeeId = employeeId;
        FullName = fullName;
        DepartmentId = departmentId;
        CategoryId = categoryId;
        DesignationId = designationId;
    }

    public virtual void SetFullName(string fullName)
    {
        FullName = fullName;
    }

    public virtual void SetDepartment(Guid? departmentId)
    {
        DepartmentId = departmentId;
    }

    public virtual void SetCategory(Guid? categoryId)
    {
        CategoryId = categoryId;
    }

    public virtual void SetDesignation(Guid? designationId)
    {
        DesignationId = designationId;
    }
}
