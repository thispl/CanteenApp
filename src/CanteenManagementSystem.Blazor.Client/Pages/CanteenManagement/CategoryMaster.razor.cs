using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Volo.Abp.AspNetCore.Components.Messages;

namespace CanteenManagementSystem.Blazor.Client.Pages.CanteenManagement;

public partial class CategoryMaster
{
    [Inject]
    protected ICategoryAppService CategoryAppService { get; set; } = null!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = null!;

    protected IReadOnlyList<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    protected long TotalCount { get; set; }

    protected CategoryListFilterDto Filter { get; set; } = new()
    {
        MaxResultCount = 20,
        SkipCount = 0
    };

    protected CreateCategoryDto NewCategory { get; set; } = new();
    protected UpdateCategoryDto EditingCategory { get; set; } = new();
    protected Guid EditingCategoryId { get; set; }

    protected Modal CreateModal { get; set; } = null!;
    protected Modal EditModal { get; set; } = null!;

    protected bool IsFirstPage => Filter.SkipCount == 0;
    protected bool IsLastPage => Filter.SkipCount + Filter.MaxResultCount >= TotalCount;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    protected async Task LoadDataAsync()
    {
        try
        {
            var result = await CategoryAppService.GetListAsync(Filter);
            Categories = result.Items;
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading categories: {ex.Message}");
        }
    }

    protected async Task RefreshData()
    {
        Filter.SkipCount = 0;
        await LoadDataAsync();
    }

    protected async Task OnFilterKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            Filter.SkipCount = 0;
            await LoadDataAsync();
        }
    }

    protected async Task GoToPreviousPage()
    {
        if (!IsFirstPage)
        {
            Filter.SkipCount = Math.Max(0, Filter.SkipCount - Filter.MaxResultCount);
            await LoadDataAsync();
        }
    }

    protected async Task GoToNextPage()
    {
        if (!IsLastPage)
        {
            Filter.SkipCount += Filter.MaxResultCount;
            await LoadDataAsync();
        }
    }

    protected void OnFilterInput(ChangeEventArgs e)
    {
        Filter.Filter = e.Value?.ToString();
        StateHasChanged();
    }

    protected void OpenCreateModal()
    {
        NewCategory = new CreateCategoryDto();
        CreateModal.Show();
    }

    protected void CloseCreateModal()
    {
        CreateModal.Hide();
    }

    protected void OpenEditModal(CategoryDto category)
    {
        EditingCategoryId = category.Id;
        EditingCategory = new UpdateCategoryDto
        {
            CategoryName = category.CategoryName,
            CategoryCode = category.CategoryCode
        };
        EditModal.Show();
    }

    protected void CloseEditModal()
    {
        EditModal.Hide();
    }

    protected async Task CreateCategoryAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCategory.CategoryName))
        {
            await UiMessageService.Error("Category name is required");
            return;
        }

        try
        {
            await CategoryAppService.CreateAsync(NewCategory);
            await UiMessageService.Success("Category created successfully");
            CloseCreateModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error creating category: {ex.Message}");
        }
    }

    protected async Task UpdateCategoryAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingCategory.CategoryName))
        {
            await UiMessageService.Error("Category name is required");
            return;
        }

        try
        {
            await CategoryAppService.UpdateAsync(EditingCategoryId, EditingCategory);
            await UiMessageService.Success("Category updated successfully");
            CloseEditModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error updating category: {ex.Message}");
        }
    }

    protected async Task DeleteCategoryAsync(CategoryDto category)
    {
        var confirmed = await UiMessageService.Confirm(
            L["ConfirmDeleteCategory"],
            "Delete Confirmation");

        if (confirmed)
        {
            try
            {
                await CategoryAppService.DeleteAsync(category.Id);
                await UiMessageService.Success("Category deleted successfully");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error($"Error deleting category: {ex.Message}");
            }
        }
    }
}
