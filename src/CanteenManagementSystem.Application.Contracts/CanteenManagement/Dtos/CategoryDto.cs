using System;
using Volo.Abp.Application.Dtos;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

/// <summary>
/// DTO for Category entity
/// </summary>
public class CategoryDto : AuditedEntityDto<Guid>
{
    public string CategoryName { get; set; } = null!;
    public string? CategoryCode { get; set; }
}

/// <summary>
/// DTO for creating a new category
/// </summary>
public class CreateCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryCode { get; set; }
}

/// <summary>
/// DTO for updating a category
/// </summary>
public class UpdateCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryCode { get; set; }
}

/// <summary>
/// DTO for filtering categories
/// </summary>
public class CategoryListFilterDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
