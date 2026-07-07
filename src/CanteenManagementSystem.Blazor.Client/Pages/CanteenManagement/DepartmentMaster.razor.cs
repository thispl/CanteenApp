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

public partial class DepartmentMaster
{
    [Inject]
    protected IDepartmentAppService DepartmentAppService { get; set; } = null!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = null!;

    protected IReadOnlyList<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
    protected long TotalCount { get; set; }

    protected DepartmentListFilterDto Filter { get; set; } = new()
    {
        MaxResultCount = 20,
        SkipCount = 0
    };

    protected CreateDepartmentDto NewDepartment { get; set; } = new();
    protected UpdateDepartmentDto EditingDepartment { get; set; } = new();
    protected Guid EditingDepartmentId { get; set; }

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
            var result = await DepartmentAppService.GetListAsync(Filter);
            Departments = result.Items;
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading departments: {ex.Message}");
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
        NewDepartment = new CreateDepartmentDto();
        CreateModal.Show();
    }

    protected void CloseCreateModal()
    {
        CreateModal.Hide();
    }

    protected void OpenEditModal(DepartmentDto department)
    {
        EditingDepartmentId = department.Id;
        EditingDepartment = new UpdateDepartmentDto
        {
            Name = department.Name,
            CCCode = department.CCCode
        };
        EditModal.Show();
    }

    protected void CloseEditModal()
    {
        EditModal.Hide();
    }

    protected async Task CreateDepartmentAsync()
    {
        if (string.IsNullOrWhiteSpace(NewDepartment.Name))
        {
            await UiMessageService.Error("Department name is required");
            return;
        }

        try
        {
            await DepartmentAppService.CreateAsync(NewDepartment);
            await UiMessageService.Success("Department created successfully");
            CloseCreateModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error creating department: {ex.Message}");
        }
    }

    protected async Task UpdateDepartmentAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingDepartment.Name))
        {
            await UiMessageService.Error("Department name is required");
            return;
        }

        try
        {
            await DepartmentAppService.UpdateAsync(EditingDepartmentId, EditingDepartment);
            await UiMessageService.Success("Department updated successfully");
            CloseEditModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error updating department: {ex.Message}");
        }
    }

    protected async Task DeleteDepartmentAsync(DepartmentDto department)
    {
        var confirmed = await UiMessageService.Confirm(
            L["ConfirmDeleteDepartment"],
            "Delete Confirmation");

        if (confirmed)
        {
            try
            {
                await DepartmentAppService.DeleteAsync(department.Id);
                await UiMessageService.Success("Department deleted successfully");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error($"Error deleting department: {ex.Message}");
            }
        }
    }
}
