using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using CanteenManagementSystem.CanteenManagement;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Volo.Abp.AspNetCore.Components.Messages;

namespace CanteenManagementSystem.Blazor.Client.Pages.CanteenManagement;

public partial class DeviceMaster
{
    [Inject]
    protected IDeviceAppService DeviceAppService { get; set; } = null!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = null!;

    protected IReadOnlyList<DeviceDto> Devices { get; set; } = new List<DeviceDto>();
    protected long TotalCount { get; set; }

    protected DeviceListFilterDto Filter { get; set; } = new()
    {
        MaxResultCount = 20,
        SkipCount = 0
    };

    protected CreateDeviceDto NewDevice { get; set; } = new();
    protected UpdateDeviceDto EditingDevice { get; set; } = new();
    protected Guid EditingDeviceId { get; set; }

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
            var result = await DeviceAppService.GetListAsync(Filter);
            Devices = result.Items;
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading devices: {ex.Message}");
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
        NewDevice = new CreateDeviceDto { Status = DeviceStatus.Active };
        CreateModal.Show();
    }

    protected void CloseCreateModal()
    {
        CreateModal.Hide();
    }

    protected void OpenEditModal(DeviceDto device)
    {
        EditingDeviceId = device.Id;
        EditingDevice = new UpdateDeviceDto
        {
            DeviceId = device.DeviceId,
            Name = device.Name,
            IpAddress = device.IpAddress,
            Port = device.Port,
            Status = device.Status,
            Location = device.Location,
            Model = device.Model,
            SerialNumber = device.SerialNumber
        };
        EditModal.Show();
    }

    protected void CloseEditModal()
    {
        EditModal.Hide();
    }

    protected async Task CreateDeviceAsync()
    {
        if (string.IsNullOrWhiteSpace(NewDevice.DeviceId) || string.IsNullOrWhiteSpace(NewDevice.Name))
        {
            await UiMessageService.Error("Device ID and Name are required.");
            return;
        }

        try
        {
            await DeviceAppService.CreateAsync(NewDevice);
            await UiMessageService.Success("Device created successfully");
            CloseCreateModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error creating device: {ex.Message}");
        }
    }

    protected async Task UpdateDeviceAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingDevice.DeviceId) || string.IsNullOrWhiteSpace(EditingDevice.Name))
        {
            await UiMessageService.Error("Device ID and Name are required.");
            return;
        }

        try
        {
            await DeviceAppService.UpdateAsync(EditingDeviceId, EditingDevice);
            await UiMessageService.Success("Device updated successfully");
            CloseEditModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error updating device: {ex.Message}");
        }
    }

    protected async Task DeleteDeviceAsync(DeviceDto device)
    {
        var confirmed = await UiMessageService.Confirm(
            L["ConfirmDeleteDevice"],
            "Delete Confirmation");

        if (confirmed)
        {
            try
            {
                await DeviceAppService.DeleteAsync(device.Id);
                await UiMessageService.Success("Device deleted successfully");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error($"Error deleting device: {ex.Message}");
            }
        }
    }
}
