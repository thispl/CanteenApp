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
    private readonly IGuidGenerator _guidGenerator;
    private readonly ZkTecoDbContext _zkTecoDbContext;

    public EmployeeAppService(
        IEmployeeRepository employeeRepository,
        IGuidGenerator guidGenerator,
        ZkTecoDbContext zkTecoDbContext)
    {
        _employeeRepository = employeeRepository;
        _guidGenerator = guidGenerator;
        _zkTecoDbContext = zkTecoDbContext;
    }

    public virtual async Task<EmployeeDto?> GetAsync(Guid id)
    {
        var employee = await _employeeRepository.GetAsync(id);
        return ObjectMapper.Map<Employee, EmployeeDto>(employee);
    }

    public virtual async Task<EmployeeDto?> GetByEmployeeIdAsync(string employeeId)
    {
        var employee = await _employeeRepository.FindByEmployeeIdAsync(employeeId);
        if (employee == null)
        {
            return null;
        }
        return ObjectMapper.Map<Employee, EmployeeDto>(employee);
    }

    public virtual async Task<PagedResultDto<EmployeeDto>> GetListAsync(EmployeeListFilterDto input)
    {
        var count = await _employeeRepository.GetCountAsync(input.Filter, input.Department);
        var employees = await _employeeRepository.GetListAsync(
            input.Filter,
            input.Department,
            true,
            CancellationToken.None);

        var totalCount = employees.Count;
        var pagedEmployees = employees
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<EmployeeDto>(
            count,
            ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(pagedEmployees));
    }

    public virtual async Task<EmployeeDto> CreateAsync(CreateEmployeeDto input)
    {
        // Check if employee with same ID already exists
        if (await _employeeRepository.ExistsByEmployeeIdAsync(input.EmployeeId))
        {
            throw new UserFriendlyException($"Employee with ID '{input.EmployeeId}' already exists.");
        }

        var employee = new Employee(
            _guidGenerator.Create(),
            input.EmployeeId,
            input.FullName,
            input.Department);

        await _employeeRepository.InsertAsync(employee);

        return ObjectMapper.Map<Employee, EmployeeDto>(employee);
    }

    public virtual async Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto input)
    {
        var employee = await _employeeRepository.GetAsync(id);

        employee.SetFullName(input.FullName);
        employee.SetDepartment(input.Department);

        await _employeeRepository.UpdateAsync(employee);

        return ObjectMapper.Map<Employee, EmployeeDto>(employee);
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
                    extEmp.Name,
                    null); // Department not available in ZKTeco

                await _employeeRepository.InsertAsync(employee, autoSave: true);
                importedCount++;
            }
        }

        return importedCount;
    }
}
