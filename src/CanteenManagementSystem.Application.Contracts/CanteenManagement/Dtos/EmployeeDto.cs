using System;
using Volo.Abp.Application.Dtos;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

/// <summary>
/// DTO for Employee entity
/// </summary>
public class EmployeeDto : AuditedEntityDto<Guid>
{
    public string EmployeeId { get; set; } = null!;
    public string FullName { get; set; } = null!;

    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }

    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public Guid? DesignationId { get; set; }
    public string? DesignationName { get; set; }
}

/// <summary>
/// DTO for creating a new employee
/// </summary>
public class CreateEmployeeDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? DesignationId { get; set; }
}

/// <summary>
/// DTO for updating an employee
/// </summary>
public class UpdateEmployeeDto
{
    public string FullName { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? DesignationId { get; set; }
}

/// <summary>
/// DTO for filtering employees
/// </summary>
public class EmployeeListFilterDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? DesignationId { get; set; }
}
