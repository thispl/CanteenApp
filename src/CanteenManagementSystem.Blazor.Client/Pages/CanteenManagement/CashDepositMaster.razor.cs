using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Services;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;

namespace CanteenManagementSystem.Blazor.Client.Pages.CanteenManagement;

public partial class CashDepositMaster
{
    [Inject]
    protected ICashDepositAppService CashDepositAppService { get; set; } = null!;

    [Inject]
    protected IEmployeeAppService EmployeeAppService { get; set; } = null!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = null!;

    protected IReadOnlyList<CashDepositDto> Deposits { get; set; } = new List<CashDepositDto>();
    protected IReadOnlyList<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    protected long TotalCount { get; set; }

    protected CashDepositListFilterDto Filter { get; set; } = new()
    {
        MaxResultCount = 20,
        SkipCount = 0
    };

    protected CreateCashDepositDto NewDeposit { get; set; } = new();
    protected UpdateCashDepositDto EditingDeposit { get; set; } = new();
    protected Guid EditingDepositId { get; set; }

    protected Modal CreateModal { get; set; } = null!;
    protected Modal EditModal { get; set; } = null!;

    protected bool IsFirstPage => Filter.SkipCount == 0;
    protected bool IsLastPage => Filter.SkipCount + Filter.MaxResultCount >= TotalCount;

    protected override async Task OnInitializedAsync()
    {
        await LoadEmployeesAsync();
        await LoadDataAsync();
    }

    protected async Task LoadEmployeesAsync()
    {
        try
        {
            var result = await EmployeeAppService.GetListAsync(new EmployeeListFilterDto { MaxResultCount = 1000 });
            Employees = result.Items;
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading employees: {ex.Message}");
        }
    }

    protected async Task LoadDataAsync()
    {
        try
        {
            var result = await CashDepositAppService.GetListAsync(Filter);
            Deposits = result.Items;
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading cash deposits: {ex.Message}");
        }
    }

    protected async Task RefreshData()
    {
        Filter.SkipCount = 0;
        Filter.EmployeeId = null;
        Filter.From = null;
        Filter.To = null;
        await LoadDataAsync();
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
        NewDeposit = new CreateCashDepositDto
        {
            DepositDate = DateTime.Today
        };
        CreateModal.Show();
    }

    protected void CloseCreateModal()
    {
        CreateModal.Hide();
    }

    protected void OpenEditModal(CashDepositDto deposit)
    {
        EditingDepositId = deposit.Id;
        EditingDeposit = new UpdateCashDepositDto
        {
            EmployeeId = deposit.EmployeeId,
            Amount = deposit.Amount,
            DepositDate = deposit.DepositDate,
            Notes = deposit.Notes
        };
        EditModal.Show();
    }

    protected void CloseEditModal()
    {
        EditModal.Hide();
    }

    protected async Task CreateDepositAsync()
    {
        if (NewDeposit.EmployeeId == Guid.Empty)
        {
            await UiMessageService.Error("Employee is required.");
            return;
        }

        if (NewDeposit.Amount <= 0)
        {
            await UiMessageService.Error("Amount must be greater than zero.");
            return;
        }

        try
        {
            await CashDepositAppService.CreateAsync(NewDeposit);
            await UiMessageService.Success("Cash deposit created successfully.");
            CloseCreateModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error creating cash deposit: {ex.Message}");
        }
    }

    protected async Task UpdateDepositAsync()
    {
        if (EditingDeposit.EmployeeId == Guid.Empty)
        {
            await UiMessageService.Error("Employee is required.");
            return;
        }

        if (EditingDeposit.Amount <= 0)
        {
            await UiMessageService.Error("Amount must be greater than zero.");
            return;
        }

        try
        {
            await CashDepositAppService.UpdateAsync(EditingDepositId, EditingDeposit);
            await UiMessageService.Success("Cash deposit updated successfully.");
            CloseEditModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error updating cash deposit: {ex.Message}");
        }
    }

    protected async Task DeleteDepositAsync(CashDepositDto deposit)
    {
        var confirmed = await UiMessageService.Confirm(
            L["ConfirmDeleteCashDeposit"],
            "Delete Confirmation");

        if (confirmed)
        {
            try
            {
                await CashDepositAppService.DeleteAsync(deposit.Id);
                await UiMessageService.Success("Cash deposit deleted successfully.");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error($"Error deleting cash deposit: {ex.Message}");
            }
        }
    }
}
