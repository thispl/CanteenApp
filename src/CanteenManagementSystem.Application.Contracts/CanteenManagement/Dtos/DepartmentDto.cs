using System;
using Volo.Abp.Application.Dtos;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

/// <summary>
/// DTO for Department entity
/// </summary>
public class DepartmentDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string? CCCode { get; set; }
}

/// <summary>
/// DTO for creating a new department
/// </summary>
public class CreateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string? CCCode { get; set; }
}

/// <summary>
/// DTO for updating a department
/// </summary>
public class UpdateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string? CCCode { get; set; }
}

/// <summary>
/// DTO for filtering departments
/// </summary>
public class DepartmentListFilterDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
