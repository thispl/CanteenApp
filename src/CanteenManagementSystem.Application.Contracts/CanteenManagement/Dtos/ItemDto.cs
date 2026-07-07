using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

/// <summary>
/// DTO for Item entity
/// </summary>
public class ItemDto : AuditedEntityDto<Guid>
{
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
}

/// <summary>
/// DTO for creating a new item
/// </summary>
public class CreateItemDto
{
    public string Description { get; set; } = string.Empty;

    [Range(0, 9999999999.99, ErrorMessage = "Price must be greater than or equal to 0.")]
    public decimal Price { get; set; }
}

/// <summary>
/// DTO for updating an item
/// </summary>
public class UpdateItemDto
{
    public string Description { get; set; } = string.Empty;

    [Range(0, 9999999999.99, ErrorMessage = "Price must be greater than or equal to 0.")]
    public decimal Price { get; set; }
}

/// <summary>
/// DTO for filtering items
/// </summary>
public class ItemListFilterDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
