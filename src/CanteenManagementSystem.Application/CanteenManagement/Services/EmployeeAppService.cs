using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using CanteenManagementSystem.EntityFrameworkCore.ZkTecoIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service for Employee management
/// </summary>
[Authorize]
public class EmployeeAppService : ApplicationService, IEmployeeAppService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDesignationRepository _designationRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ZkTecoDbContext _zkTecoDbContext;

    public EmployeeAppService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        ICategoryRepository categoryRepository,
        IDesignationRepository designationRepository,
        IGuidGenerator guidGenerator,
        ZkTecoDbContext zkTecoDbContext)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _categoryRepository = categoryRepository;
        _designationRepository = designationRepository;
        _guidGenerator = guidGenerator;
        _zkTecoDbContext = zkTecoDbContext;
    }

    public virtual async Task<EmployeeDto?> GetAsync(Guid id)
    {
        var employee = await _employeeRepository.GetAsync(id);
        return MapWithRelatedNames(employee);
    }

    public virtual async Task<EmployeeDto?> GetByEmployeeIdAsync(string employeeId)
    {
        var employee = await _employeeRepository.FindByEmployeeIdAsync(employeeId);
        if (employee == null)
        {
            return null;
        }
        return MapWithRelatedNames(employee);
    }

    public virtual async Task<PagedResultDto<EmployeeDto>> GetListAsync(EmployeeListFilterDto input)
    {
        var count = await _employeeRepository.GetCountAsync(
            input.Filter,
            input.DepartmentId,
            input.CategoryId,
            input.DesignationId);
        var employees = await _employeeRepository.GetListAsync(
            input.Filter,
            input.DepartmentId,
            input.CategoryId,
            input.DesignationId,
            true,
            CancellationToken.None);

        var pagedEmployees = employees
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<EmployeeDto>(
            count,
            pagedEmployees.Select(MapWithRelatedNames).ToList());
    }

    public virtual async Task<EmployeeDto> CreateAsync(CreateEmployeeDto input)
    {
        // Check if employee with same ID already exists
        if (await _employeeRepository.ExistsByEmployeeIdAsync(input.EmployeeId))
        {
            throw new UserFriendlyException($"Employee with ID '{input.EmployeeId}' already exists.");
        }

        await ValidateForeignKeysAsync(input.DepartmentId, input.CategoryId, input.DesignationId);

        var employee = new Employee(
            _guidGenerator.Create(),
            input.EmployeeId,
            input.FullName,
            input.DepartmentId,
            input.CategoryId,
            input.DesignationId);

        await _employeeRepository.InsertAsync(employee);
        await UnitOfWorkManager.Current.SaveChangesAsync();

        var createdEmployee = await _employeeRepository.GetAsync(employee.Id);
        return MapWithRelatedNames(createdEmployee);
    }

    public virtual async Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto input)
    {
        var employee = await _employeeRepository.GetAsync(id);

        await ValidateForeignKeysAsync(input.DepartmentId, input.CategoryId, input.DesignationId);

        employee.SetFullName(input.FullName);
        employee.SetDepartment(input.DepartmentId);
        employee.SetCategory(input.CategoryId);
        employee.SetDesignation(input.DesignationId);

        await _employeeRepository.UpdateAsync(employee);
        await UnitOfWorkManager.Current.SaveChangesAsync();

        var updatedEmployee = await _employeeRepository.GetAsync(employee.Id);
        return MapWithRelatedNames(updatedEmployee);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _employeeRepository.DeleteAsync(id);
    }

    public virtual async Task<bool> ExistsByEmployeeIdAsync(string employeeId)
    {
        return await _employeeRepository.ExistsByEmployeeIdAsync(employeeId);
    }

    /// <summary>
    /// Bulk imports all employees from ZKTeco personnel_employee table
    /// </summary>
    public virtual async Task<int> SyncAllEmployeesFromZkTecoAsync()
    {
        // Get all external employees
        var externalEmployees = await _zkTecoDbContext.PersonnelEmployees
            .AsNoTracking()
            .ToListAsync();

        int importedCount = 0;

        foreach (var extEmp in externalEmployees)
        {
            // Check if employee already exists
            var exists = await _employeeRepository.ExistsByEmployeeIdAsync(extEmp.EnrollNumber);
            if (!exists)
            {
                var employee = new Employee(
                    _guidGenerator.Create(),
                    extEmp.EnrollNumber,
                    extEmp.Name); // Department/Category/Designation not available in ZKTeco

                await _employeeRepository.InsertAsync(employee, autoSave: true);
                importedCount++;
            }
        }

        return importedCount;
    }

    private async Task ValidateForeignKeysAsync(Guid? departmentId, Guid? categoryId, Guid? designationId)
    {
        if (departmentId.HasValue && !await (await _departmentRepository.GetQueryableAsync()).AnyAsync(d => d.Id == departmentId.Value))
        {
            throw new UserFriendlyException($"Department with ID '{departmentId.Value}' does not exist.");
        }

        if (categoryId.HasValue && !await (await _categoryRepository.GetQueryableAsync()).AnyAsync(c => c.Id == categoryId.Value))
        {
            throw new UserFriendlyException($"Category with ID '{categoryId.Value}' does not exist.");
        }

        if (designationId.HasValue && !await (await _designationRepository.GetQueryableAsync()).AnyAsync(d => d.Id == designationId.Value))
        {
            throw new UserFriendlyException($"Designation with ID '{designationId.Value}' does not exist.");
        }
    }

    private EmployeeDto MapWithRelatedNames(Employee employee)
    {
        var dto = ObjectMapper.Map<Employee, EmployeeDto>(employee);
        dto.DepartmentName = employee.Department?.Name;
        dto.CategoryName = employee.Category?.CategoryName;
        dto.DesignationName = employee.Designation?.Title;
        return dto;
    }
}
