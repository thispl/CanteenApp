using System;
using System.Collections.Generic;
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
/// Application service for Designation management
/// </summary>
[Authorize]
public class DesignationAppService : ApplicationService, IDesignationAppService
{
    private readonly IDesignationRepository _designationRepository;
    private readonly IGuidGenerator _guidGenerator;

    public DesignationAppService(
        IDesignationRepository designationRepository,
        IGuidGenerator guidGenerator)
    {
        _designationRepository = designationRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<DesignationDto?> GetAsync(Guid id)
    {
        var designation = await _designationRepository.GetAsync(id);
        return ObjectMapper.Map<Designation, DesignationDto>(designation);
    }

    public virtual async Task<DesignationDto?> GetByCodeAsync(string code)
    {
        var designation = await _designationRepository.FindByCodeAsync(code);
        if (designation == null)
        {
            return null;
        }
        return ObjectMapper.Map<Designation, DesignationDto>(designation);
    }

    public virtual async Task<PagedResultDto<DesignationDto>> GetListAsync(DesignationListFilterDto input)
    {
        var count = await _designationRepository.GetCountAsync(input.Filter);
        var designations = await _designationRepository.GetListAsync(
            input.Filter,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            CancellationToken.None);

        return new PagedResultDto<DesignationDto>(
            count,
            ObjectMapper.Map<List<Designation>, List<DesignationDto>>(designations));
    }

    public virtual async Task<DesignationDto> CreateAsync(CreateDesignationDto input)
    {
        if (!string.IsNullOrWhiteSpace(input.Code) &&
            await _designationRepository.ExistsByCodeAsync(input.Code))
        {
            throw new UserFriendlyException($"Designation with code '{input.Code}' already exists.");
        }

        var designation = new Designation(
            _guidGenerator.Create(),
            input.Title,
            input.Code,
            input.Description);

        await _designationRepository.InsertAsync(designation);

        return ObjectMapper.Map<Designation, DesignationDto>(designation);
    }

    public virtual async Task<DesignationDto> UpdateAsync(Guid id, UpdateDesignationDto input)
    {
        var designation = await _designationRepository.GetAsync(id);

        if (!string.IsNullOrWhiteSpace(input.Code) &&
            input.Code != designation.Code &&
            await _designationRepository.ExistsByCodeAsync(input.Code))
        {
            throw new UserFriendlyException($"Designation with code '{input.Code}' already exists.");
        }

        designation.SetTitle(input.Title);
        designation.SetCode(input.Code);
        designation.SetDescription(input.Description);

        await _designationRepository.UpdateAsync(designation);

        return ObjectMapper.Map<Designation, DesignationDto>(designation);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _designationRepository.DeleteAsync(id);
    }

    public virtual async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _designationRepository.ExistsByCodeAsync(code);
    }
}
