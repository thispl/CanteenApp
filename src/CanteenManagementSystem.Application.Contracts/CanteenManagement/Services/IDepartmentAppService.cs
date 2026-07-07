using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service interface for Department management
/// </summary>
public interface IDepartmentAppService : IApplicationService
{
    /// <summary>
    /// Get department by ID
    /// </summary>
    Task<DepartmentDto?> GetAsync(Guid id);

    /// <summary>
    /// Get department by cost-center code
    /// </summary>
    Task<DepartmentDto?> GetByCCCodeAsync(string ccCode);

    /// <summary>
    /// Get list of departments with filtering
    /// </summary>
    Task<PagedResultDto<DepartmentDto>> GetListAsync(DepartmentListFilterDto input);

    /// <summary>
    /// Create a new department
    /// </summary>
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto input);

    /// <summary>
    /// Update a department
    /// </summary>
    Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input);

    /// <summary>
    /// Delete a department
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Check if department exists by cost-center code
    /// </summary>
    Task<bool> ExistsByCCCodeAsync(string ccCode);
}
