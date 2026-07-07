using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service interface for Category management
/// </summary>
public interface ICategoryAppService : IApplicationService
{
    /// <summary>
    /// Get category by ID
    /// </summary>
    Task<CategoryDto?> GetAsync(Guid id);

    /// <summary>
    /// Get category by code
    /// </summary>
    Task<CategoryDto?> GetByCodeAsync(string categoryCode);

    /// <summary>
    /// Get list of categories with filtering
    /// </summary>
    Task<PagedResultDto<CategoryDto>> GetListAsync(CategoryListFilterDto input);

    /// <summary>
    /// Create a new category
    /// </summary>
    Task<CategoryDto> CreateAsync(CreateCategoryDto input);

    /// <summary>
    /// Update a category
    /// </summary>
    Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto input);

    /// <summary>
    /// Delete a category
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Check if category exists by code
    /// </summary>
    Task<bool> ExistsByCodeAsync(string categoryCode);
}
