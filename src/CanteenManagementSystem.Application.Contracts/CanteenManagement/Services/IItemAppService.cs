using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service interface for Item management
/// </summary>
public interface IItemAppService : IApplicationService
{
    /// <summary>
    /// Get item by ID
    /// </summary>
    Task<ItemDto?> GetAsync(Guid id);

    /// <summary>
    /// Get list of items with filtering
    /// </summary>
    Task<PagedResultDto<ItemDto>> GetListAsync(ItemListFilterDto input);

    /// <summary>
    /// Create a new item
    /// </summary>
    Task<ItemDto> CreateAsync(CreateItemDto input);

    /// <summary>
    /// Update an item
    /// </summary>
    Task<ItemDto> UpdateAsync(Guid id, UpdateItemDto input);

    /// <summary>
    /// Delete an item
    /// </summary>
    Task DeleteAsync(Guid id);
}
