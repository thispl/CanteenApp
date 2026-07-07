using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IGuidGenerator _guidGenerator;

    public DepartmentAppService(
        IDepartmentRepository departmentRepository,
        IGuidGenerator guidGenerator)
    {
        _departmentRepository = departmentRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<DepartmentDto?> GetAsync(Guid id)
    {
        var department = await _departmentRepository.GetAsync(id);
        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    public virtual async Task<DepartmentDto?> GetByCCCodeAsync(string ccCode)
    {
        var department = await _departmentRepository.FindByCCCodeAsync(ccCode);
        if (department == null)
        {
            return null;
        }
        return ObjectMapper.Map<Department, DepartmentDto>(department);
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
            ObjectMapper.Map<List<Department>, List<DepartmentDto>>(pagedDepartments));
    }

    public virtual async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        if (!string.IsNullOrWhiteSpace(input.CCCode) &&
            await _departmentRepository.ExistsByCCCodeAsync(input.CCCode))
        {
            throw new UserFriendlyException($"Department with cost-center code '{input.CCCode}' already exists.");
        }

        var department = new Department(
            _guidGenerator.Create(),
            input.Name,
            input.CCCode);

        await _departmentRepository.InsertAsync(department);

        return ObjectMapper.Map<Department, DepartmentDto>(department);
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

        department.SetName(input.Name);
        department.SetCCCode(input.CCCode);

        await _departmentRepository.UpdateAsync(department);

        return ObjectMapper.Map<Department, DepartmentDto>(department);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _departmentRepository.DeleteAsync(id);
    }

    public virtual async Task<bool> ExistsByCCCodeAsync(string ccCode)
    {
        return await _departmentRepository.ExistsByCCCodeAsync(ccCode);
    }
}
