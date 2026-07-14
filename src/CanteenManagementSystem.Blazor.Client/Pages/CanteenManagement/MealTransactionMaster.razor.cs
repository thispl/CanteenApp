using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using CanteenManagementSystem.CanteenManagement;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Services;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;

namespace CanteenManagementSystem.Blazor.Client.Pages.CanteenManagement;

public partial class MealTransactionMaster
{
    [Inject]
    protected IMealTransactionAppService MealTransactionAppService { get; set; } = null!;

    [Inject]
    protected IEmployeeAppService EmployeeAppService { get; set; } = null!;

    [Inject]
    protected IDeviceAppService DeviceAppService { get; set; } = null!;

    [Inject]
    protected ITimeScheduleAppService TimeScheduleAppService { get; set; } = null!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = null!;

    protected IReadOnlyList<MealTransactionDto> Transactions { get; set; } = new List<MealTransactionDto>();
    protected IReadOnlyList<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    protected IReadOnlyList<DeviceDto> Devices { get; set; } = new List<DeviceDto>();
    protected IReadOnlyList<TimeScheduleDto> TimeSchedules { get; set; } = new List<TimeScheduleDto>();
    protected long TotalCount { get; set; }

    protected MealTransactionListFilterDto Filter { get; set; } = new()
    {
        MaxResultCount = 20,
        SkipCount = 0
    };

    protected CreateMealTransactionDto NewTransaction { get; set; } = new();
    protected UpdateMealTransactionDto EditingTransaction { get; set; } = new();
    protected Guid EditingTransactionId { get; set; }

    protected Modal CreateModal { get; set; } = null!;
    protected Modal EditModal { get; set; } = null!;

    protected bool IsFirstPage => Filter.SkipCount == 0;
    protected bool IsLastPage => Filter.SkipCount + Filter.MaxResultCount >= TotalCount;

    protected override async Task OnInitializedAsync()
    {
        await LoadReferenceDataAsync();
        await LoadDataAsync();
    }

    protected async Task LoadReferenceDataAsync()
    {
        try
        {
            var employees = await EmployeeAppService.GetListAsync(new EmployeeListFilterDto { MaxResultCount = 1000 });
            Employees = employees.Items;

            var devices = await DeviceAppService.GetListAsync(new DeviceListFilterDto { MaxResultCount = 1000 });
            Devices = devices.Items;

            var schedules = await TimeScheduleAppService.GetListAsync(new TimeScheduleListFilterDto { MaxResultCount = 1000 });
            TimeSchedules = schedules.Items.Where(s => s.ItemId.HasValue).ToList();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading reference data: {ex.Message}");
        }
    }

    protected async Task LoadDataAsync()
    {
        try
        {
            var result = await MealTransactionAppService.GetListAsync(Filter);
            Transactions = result.Items;
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading meal transactions: {ex.Message}");
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
        NewTransaction = new CreateMealTransactionDto
        {
            PunchTime = DateTime.Now,
            Source = MealTransactionSource.ManualEntry
        };
        CreateModal.Show();
    }

    protected void CloseCreateModal()
    {
        CreateModal.Hide();
    }

    protected async Task CreateTransactionAsync()
    {
        if (NewTransaction.EmployeeId == Guid.Empty)
        {
            await UiMessageService.Error("Employee is required.");
            return;
        }

        if (NewTransaction.TimeScheduleId == Guid.Empty)
        {
            await UiMessageService.Error("Time schedule is required.");
            return;
        }

        if (NewTransaction.DeviceId == Guid.Empty)
        {
            await UiMessageService.Error("Device is required.");
            return;
        }

        try
        {
            await MealTransactionAppService.CreateAsync(NewTransaction);
            await UiMessageService.Success("Manual punch created successfully.");
            CloseCreateModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error creating transaction: {ex.Message}");
        }
    }

    protected void OpenEditModal(MealTransactionDto transaction)
    {
        EditingTransactionId = transaction.Id;
        EditingTransaction = new UpdateMealTransactionDto
        {
            EmployeeId = transaction.EmployeeId,
            DeviceId = transaction.DeviceId,
            TimeScheduleId = transaction.TimeScheduleId,
            PunchTime = transaction.PunchTime
        };
        EditModal.Show();
    }

    protected void CloseEditModal()
    {
        EditModal.Hide();
    }

    protected async Task UpdateTransactionAsync()
    {
        if (EditingTransaction.EmployeeId == Guid.Empty)
        {
            await UiMessageService.Error("Employee is required.");
            return;
        }

        if (EditingTransaction.TimeScheduleId == Guid.Empty)
        {
            await UiMessageService.Error("Time schedule is required.");
            return;
        }

        if (EditingTransaction.DeviceId == Guid.Empty)
        {
            await UiMessageService.Error("Device is required.");
            return;
        }

        try
        {
            await MealTransactionAppService.UpdateAsync(EditingTransactionId, EditingTransaction);
            await UiMessageService.Success("Transaction updated successfully.");
            CloseEditModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error updating transaction: {ex.Message}");
        }
    }

    protected async Task DeleteTransactionAsync(MealTransactionDto transaction)
    {
        var confirmed = await UiMessageService.Confirm(
            L["ConfirmDeleteMealTransaction"],
            "Delete Confirmation");

        if (confirmed)
        {
            try
            {
                await MealTransactionAppService.DeleteAsync(transaction.Id);
                await UiMessageService.Success("Transaction deleted successfully.");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error($"Error deleting transaction: {ex.Message}");
            }
        }
    }
}
