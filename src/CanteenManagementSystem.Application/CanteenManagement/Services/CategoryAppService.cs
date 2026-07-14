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
/// Application service for Category management
/// </summary>
[Authorize]
public class CategoryAppService : ApplicationService, ICategoryAppService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IGuidGenerator _guidGenerator;

    public CategoryAppService(
        ICategoryRepository categoryRepository,
        IEmployeeRepository employeeRepository,
        IGuidGenerator guidGenerator)
    {
        _categoryRepository = categoryRepository;
        _employeeRepository = employeeRepository;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<CategoryDto?> GetAsync(Guid id)
    {
        var category = await _categoryRepository.GetAsync(id);
        return ObjectMapper.Map<Category, CategoryDto>(category);
    }

    public virtual async Task<CategoryDto?> GetByCodeAsync(string categoryCode)
    {
        var category = await _categoryRepository.FindByCodeAsync(categoryCode);
        if (category == null)
        {
            return null;
        }
        return ObjectMapper.Map<Category, CategoryDto>(category);
    }

    public virtual async Task<PagedResultDto<CategoryDto>> GetListAsync(CategoryListFilterDto input)
    {
        var count = await _categoryRepository.GetCountAsync(input.Filter);
        var categories = await _categoryRepository.GetListAsync(
            input.Filter,
            CancellationToken.None);

        var pagedCategories = categories
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<CategoryDto>(
            count,
            ObjectMapper.Map<List<Category>, List<CategoryDto>>(pagedCategories));
    }

    public virtual async Task<CategoryDto> CreateAsync(CreateCategoryDto input)
    {
        if (!string.IsNullOrWhiteSpace(input.CategoryCode) &&
            await _categoryRepository.ExistsByCodeAsync(input.CategoryCode))
        {
            throw new UserFriendlyException($"Category with code '{input.CategoryCode}' already exists.");
        }

        var category = new Category(
            _guidGenerator.Create(),
            input.CategoryName,
            input.CategoryCode);

        await _categoryRepository.InsertAsync(category);

        return ObjectMapper.Map<Category, CategoryDto>(category);
    }

    public virtual async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto input)
    {
        var category = await _categoryRepository.GetAsync(id);

        if (!string.IsNullOrWhiteSpace(input.CategoryCode) &&
            input.CategoryCode != category.CategoryCode &&
            await _categoryRepository.ExistsByCodeAsync(input.CategoryCode))
        {
            throw new UserFriendlyException($"Category with code '{input.CategoryCode}' already exists.");
        }

        category.SetCategoryName(input.CategoryName);
        category.SetCategoryCode(input.CategoryCode);

        await _categoryRepository.UpdateAsync(category);

        return ObjectMapper.Map<Category, CategoryDto>(category);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var employeeCount = await _employeeRepository.GetCountAsync(categoryId: id);
        if (employeeCount > 0)
        {
            throw new UserFriendlyException("Cannot delete this category because it is assigned to one or more employees.");
        }

        await _categoryRepository.DeleteAsync(id);
    }

    public virtual async Task<bool> ExistsByCodeAsync(string categoryCode)
    {
        return await _categoryRepository.ExistsByCodeAsync(categoryCode);
    }
}
