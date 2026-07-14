using System;
using System.Collections.Generic;
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
/// Application service for Company management
/// </summary>
[Authorize]
public class CompanyAppService : ApplicationService, ICompanyAppService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IGuidGenerator _guidGenerator;

    public CompanyAppService(
        ICompanyRepository companyRepository,
        IDepartmentRepository departmentRepository,
        IGuidGenerator guidGenerator)
    {
        _companyRepository = companyRepository;
        _departmentRepository = departmentRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<CompanyDto?> GetAsync(Guid id)
    {
        var company = await _companyRepository.GetAsync(id);
        return ObjectMapper.Map<Company, CompanyDto>(company);
    }

    public virtual async Task<CompanyDto?> GetByCodeAsync(string code)
    {
        var company = await _companyRepository.FindByCodeAsync(code);
        if (company == null)
        {
            return null;
        }
        return ObjectMapper.Map<Company, CompanyDto>(company);
    }

    public virtual async Task<PagedResultDto<CompanyDto>> GetListAsync(CompanyListFilterDto input)
    {
        var count = await _companyRepository.GetCountAsync(input.Filter);
        var companies = await _companyRepository.GetListAsync(
            input.Filter,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            CancellationToken.None);

        return new PagedResultDto<CompanyDto>(
            count,
            ObjectMapper.Map<List<Company>, List<CompanyDto>>(companies));
    }

    public virtual async Task<CompanyDto> CreateAsync(CreateCompanyDto input)
    {
        if (!string.IsNullOrWhiteSpace(input.Code) &&
            await _companyRepository.ExistsByCodeAsync(input.Code))
        {
            throw new UserFriendlyException($"Company with code '{input.Code}' already exists.");
        }

        var company = new Company(
            _guidGenerator.Create(),
            input.Name,
            input.Code,
            input.Address,
            input.Phone,
            input.Email,
            input.TaxNumber,
            input.Website);

        await _companyRepository.InsertAsync(company);

        return ObjectMapper.Map<Company, CompanyDto>(company);
    }

    public virtual async Task<CompanyDto> UpdateAsync(Guid id, UpdateCompanyDto input)
    {
        var company = await _companyRepository.GetAsync(id);

        if (!string.IsNullOrWhiteSpace(input.Code) &&
            input.Code != company.Code &&
            await _companyRepository.ExistsByCodeAsync(input.Code))
        {
            throw new UserFriendlyException($"Company with code '{input.Code}' already exists.");
        }

        company.SetName(input.Name);
        company.SetCode(input.Code);
        company.SetAddress(input.Address);
        company.SetPhone(input.Phone);
        company.SetEmail(input.Email);
        company.SetTaxNumber(input.TaxNumber);
        company.SetWebsite(input.Website);

        await _companyRepository.UpdateAsync(company);

        return ObjectMapper.Map<Company, CompanyDto>(company);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        if (await (await _departmentRepository.GetQueryableAsync()).AnyAsync(d => d.CompanyId == id))
        {
            throw new UserFriendlyException("Cannot delete this company because it is assigned to one or more departments.");
        }

        await _companyRepository.DeleteAsync(id);
    }

    public virtual async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _companyRepository.ExistsByCodeAsync(code);
    }
}
