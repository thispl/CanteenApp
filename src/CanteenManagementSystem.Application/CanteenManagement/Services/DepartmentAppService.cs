using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Guids;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service for Department management
/// </summary>
[Authorize]
public class DepartmentAppService : ApplicationService, IDepartmentAppService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IGuidGenerator _guidGenerator;

    public DepartmentAppService(
        IDepartmentRepository departmentRepository,
        ICompanyRepository companyRepository,
        IEmployeeRepository employeeRepository,
        IGuidGenerator guidGenerator)
    {
        _departmentRepository = departmentRepository;
        _companyRepository = companyRepository;
        _employeeRepository = employeeRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<DepartmentDto?> GetAsync(Guid id)
    {
        var department = await _departmentRepository.GetAsync(id);
        return MapWithCompanyName(department);
    }

    public virtual async Task<DepartmentDto?> GetByCCCodeAsync(string ccCode)
    {
        var department = await _departmentRepository.FindByCCCodeAsync(ccCode);
        if (department == null)
        {
            return null;
        }
        return MapWithCompanyName(department);
    }

    public virtual async Task<PagedResultDto<DepartmentDto>> GetListAsync(DepartmentListFilterDto input)
    {
        var count = await _departmentRepository.GetCountAsync(input.Filter);
        var departments = await _departmentRepository.GetListAsync(
            input.Filter,
            CancellationToken.None);

        var pagedDepartments = departments
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<DepartmentDto>(
            count,
            pagedDepartments.Select(MapWithCompanyName).ToList());
    }

    public virtual async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        if (!string.IsNullOrWhiteSpace(input.CCCode) &&
            await _departmentRepository.ExistsByCCCodeAsync(input.CCCode))
        {
            throw new UserFriendlyException($"Department with cost-center code '{input.CCCode}' already exists.");
        }

        await ValidateCompanyAsync(input.CompanyId);

        var department = new Department(
            _guidGenerator.Create(),
            input.Name,
            input.CCCode,
            input.CompanyId);

        await _departmentRepository.InsertAsync(department);
        await UnitOfWorkManager.Current.SaveChangesAsync();

        var createdDepartment = await _departmentRepository.GetAsync(department.Id);
        return MapWithCompanyName(createdDepartment);
    }

    public virtual async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input)
    {
        var department = await _departmentRepository.GetAsync(id);

        if (!string.IsNullOrWhiteSpace(input.CCCode) &&
            input.CCCode != department.CCCode &&
            await _departmentRepository.ExistsByCCCodeAsync(input.CCCode))
        {
            throw new UserFriendlyException($"Department with cost-center code '{input.CCCode}' already exists.");
        }

        await ValidateCompanyAsync(input.CompanyId);

        department.SetName(input.Name);
        department.SetCCCode(input.CCCode);
        department.SetCompany(input.CompanyId);

        await _departmentRepository.UpdateAsync(department);
        await UnitOfWorkManager.Current.SaveChangesAsync();

        var updatedDepartment = await _departmentRepository.GetAsync(department.Id);
        return MapWithCompanyName(updatedDepartment);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var employeeCount = await _employeeRepository.GetCountAsync(departmentId: id);
        if (employeeCount > 0)
        {
            throw new UserFriendlyException("Cannot delete this department because it is assigned to one or more employees.");
        }

        await _departmentRepository.DeleteAsync(id);
    }

    public virtual async Task<bool> ExistsByCCCodeAsync(string ccCode)
    {
        return await _departmentRepository.ExistsByCCCodeAsync(ccCode);
    }

    private async Task ValidateCompanyAsync(Guid? companyId)
    {
        if (companyId.HasValue && !await (await _companyRepository.GetQueryableAsync()).AnyAsync(c => c.Id == companyId.Value))
        {
            throw new UserFriendlyException($"Company with ID '{companyId.Value}' does not exist.");
        }
    }

    private DepartmentDto MapWithCompanyName(Department department)
    {
        var dto = ObjectMapper.Map<Department, DepartmentDto>(department);
        dto.CompanyName = department.Company?.Name;
        return dto;
    }
}
