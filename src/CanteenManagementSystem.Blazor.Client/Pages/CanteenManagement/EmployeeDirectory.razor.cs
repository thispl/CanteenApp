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

public partial class EmployeeDirectory
{
    [Inject]
    protected IEmployeeAppService EmployeeAppService { get; set; } = null!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = null!;

    protected IReadOnlyList<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    protected long TotalCount { get; set; }

    protected EmployeeListFilterDto Filter { get; set; } = new()
    {
        MaxResultCount = 20,
        SkipCount = 0
    };

    protected CreateEmployeeDto NewEmployee { get; set; } = new();
    protected UpdateEmployeeDto EditingEmployee { get; set; } = new();
    protected Guid EditingEmployeeId { get; set; }

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
            var result = await EmployeeAppService.GetListAsync(Filter);
            Employees = result.Items;
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading employees: {ex.Message}");
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

    protected void OpenCreateModal()
    {
        NewEmployee = new CreateEmployeeDto();
        CreateModal.Show();
    }

    protected void CloseCreateModal()
    {
        CreateModal.Hide();
    }

    protected void OpenEditModal(EmployeeDto employee)
    {
        EditingEmployeeId = employee.Id;
        EditingEmployee = new UpdateEmployeeDto
        {
            FullName = employee.FullName,
            Department = employee.Department
        };
        EditModal.Show();
    }

    protected void CloseEditModal()
    {
        EditModal.Hide();
    }

    protected void OnFilterInput(ChangeEventArgs e)
    {
        Filter.Filter = e.Value?.ToString();
        StateHasChanged();
    }

    protected async Task CreateEmployeeAsync()
    {
        if (string.IsNullOrWhiteSpace(NewEmployee.EmployeeId) || string.IsNullOrWhiteSpace(NewEmployee.FullName))
        {
            await UiMessageService.Error("EmployeeId and FullName are required.");
            return;
        }

        try
        {
            await EmployeeAppService.CreateAsync(NewEmployee);
            await UiMessageService.Success("Employee created successfully");
            CloseCreateModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error creating employee: {ex.Message}");
        }
    }

    protected async Task UpdateEmployeeAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingEmployee.FullName))
        {
            await UiMessageService.Error("FullName is required.");
            return;
        }

        try
        {
            await EmployeeAppService.UpdateAsync(EditingEmployeeId, EditingEmployee);
            await UiMessageService.Success("Employee updated successfully");
            CloseEditModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error updating employee: {ex.Message}");
        }
    }

    protected async Task DeleteEmployeeAsync(EmployeeDto employee)
    {
        var confirmed = await UiMessageService.Confirm(
            $"Are you sure you want to delete employee '{employee.FullName}'?",
            "Delete Confirmation");

        if (confirmed)
        {
            try
            {
                await EmployeeAppService.DeleteAsync(employee.Id);
                await UiMessageService.Success("Employee deleted successfully");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error($"Error deleting employee: {ex.Message}");
            }
        }
    }
}
