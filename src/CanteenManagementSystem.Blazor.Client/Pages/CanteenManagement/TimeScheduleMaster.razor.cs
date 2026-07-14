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

public partial class TimeScheduleMaster
{
    [Inject]
    protected ITimeScheduleAppService TimeScheduleAppService { get; set; } = null!;

    [Inject]
    protected IItemAppService ItemAppService { get; set; } = null!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = null!;

    protected IReadOnlyList<TimeScheduleDto> TimeSchedules { get; set; } = new List<TimeScheduleDto>();
    protected IReadOnlyList<ItemDto> Items { get; set; } = new List<ItemDto>();
    protected long TotalCount { get; set; }

    protected TimeScheduleListFilterDto Filter { get; set; } = new()
    {
        MaxResultCount = 20,
        SkipCount = 0
    };

    protected CreateTimeScheduleDto NewTimeSchedule { get; set; } = new();
    protected UpdateTimeScheduleDto EditingTimeSchedule { get; set; } = new();
    protected Guid EditingTimeScheduleId { get; set; }

    protected Modal CreateModal { get; set; } = null!;
    protected Modal EditModal { get; set; } = null!;

    protected bool IsFirstPage => Filter.SkipCount == 0;
    protected bool IsLastPage => Filter.SkipCount + Filter.MaxResultCount >= TotalCount;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        await LoadItemsAsync();
    }

    protected async Task LoadDataAsync()
    {
        try
        {
            var result = await TimeScheduleAppService.GetListAsync(Filter);
            TimeSchedules = result.Items;
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading time schedules: {ex.Message}");
        }
    }

    protected async Task LoadItemsAsync()
    {
        try
        {
            var result = await ItemAppService.GetListAsync(new ItemListFilterDto { MaxResultCount = 1000 });
            Items = result.Items;
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading items: {ex.Message}");
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
        NewTimeSchedule = new CreateTimeScheduleDto
        {
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(10, 0)
        };
        CreateModal.Show();
    }

    protected void CloseCreateModal()
    {
        CreateModal.Hide();
    }

    protected void OpenEditModal(TimeScheduleDto timeSchedule)
    {
        EditingTimeScheduleId = timeSchedule.Id;
        EditingTimeSchedule = new UpdateTimeScheduleDto
        {
            Name = timeSchedule.Name,
            Code = timeSchedule.Code,
            StartTime = timeSchedule.StartTime,
            EndTime = timeSchedule.EndTime,
            ItemId = timeSchedule.ItemId
        };
        EditModal.Show();
    }

    protected void CloseEditModal()
    {
        EditModal.Hide();
    }

    protected async Task CreateTimeScheduleAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTimeSchedule.Name))
        {
            await UiMessageService.Error("Name is required");
            return;
        }

        if (NewTimeSchedule.ItemId == Guid.Empty)
        {
            await UiMessageService.Error("Item is required");
            return;
        }

        try
        {
            await TimeScheduleAppService.CreateAsync(NewTimeSchedule);
            await UiMessageService.Success("Time schedule created successfully");
            CloseCreateModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error creating time schedule: {ex.Message}");
        }
    }

    protected async Task UpdateTimeScheduleAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingTimeSchedule.Name))
        {
            await UiMessageService.Error("Name is required");
            return;
        }

        if (EditingTimeSchedule.ItemId == Guid.Empty)
        {
            await UiMessageService.Error("Item is required");
            return;
        }

        try
        {
            await TimeScheduleAppService.UpdateAsync(EditingTimeScheduleId, EditingTimeSchedule);
            await UiMessageService.Success("Time schedule updated successfully");
            CloseEditModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error updating time schedule: {ex.Message}");
        }
    }

    protected async Task DeleteTimeScheduleAsync(TimeScheduleDto timeSchedule)
    {
        var confirmed = await UiMessageService.Confirm(
            L["ConfirmDeleteTimeSchedule"],
            "Delete Confirmation");

        if (confirmed)
        {
            try
            {
                await TimeScheduleAppService.DeleteAsync(timeSchedule.Id);
                await UiMessageService.Success("Time schedule deleted successfully");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error($"Error deleting time schedule: {ex.Message}");
            }
        }
    }
}
