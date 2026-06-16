using System;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service interface for Employee management
/// </summary>
public interface IEmployeeAppService : IApplicationService
{
    /// <summary>
    /// Get employee by ID
    /// </summary>
    Task<EmployeeDto?> GetAsync(Guid id);

    /// <summary>
    /// Get employee by EmployeeId (business identifier)
    /// </summary>
    Task<EmployeeDto?> GetByEmployeeIdAsync(string employeeId);

    /// <summary>
    /// Get list of employees with filtering
    /// </summary>
    Task<PagedResultDto<EmployeeDto>> GetListAsync(EmployeeListFilterDto input);

    /// <summary>
    /// Create a new employee
    /// </summary>
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto input);

    /// <summary>
    /// Update an employee
    /// </summary>
    Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto input);

    /// <summary>
    /// Delete an employee
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Check if employee exists by EmployeeId
    /// </summary>
    Task<bool> ExistsByEmployeeIdAsync(string employeeId);

    /// <summary>
    /// Bulk import all employees from ZKTeco database
    /// </summary>
    Task<int> SyncAllEmployeesFromZkTecoAsync();
}
