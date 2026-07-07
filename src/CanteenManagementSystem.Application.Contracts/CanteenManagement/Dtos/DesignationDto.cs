using System;
using Volo.Abp.Application.Dtos;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

/// <summary>
/// DTO for Designation entity
/// </summary>
public class DesignationDto : AuditedEntityDto<Guid>
{
    public string Title { get; set; } = null!;
    public string? Code { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO for creating a new designation
/// </summary>
public class CreateDesignationDto
{
    public string Title { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating a designation
/// </summary>
public class UpdateDesignationDto
{
    public string Title { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO for filtering designations
/// </summary>
public class DesignationListFilterDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
