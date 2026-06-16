using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using CanteenManagementSystem.CanteenManagement.Services;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;

namespace CanteenManagementSystem.Blazor.Client.Pages.CanteenManagement;

public partial class LiveCanteenLogs
{
    [Inject]
    protected ICanteenCheckInAppService CheckInAppService { get; set; } = null!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = null!;

    protected List<CanteenCheckInDto> CheckIns { get; set; } = new();
    protected bool IsLoading { get; set; }

    private Timer? _refreshTimer;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();

        // Auto-refresh every 10 seconds
        _refreshTimer = new Timer(async _ =>
        {
            await InvokeAsync(async () =>
            {
                await LoadDataAsync();
                StateHasChanged();
            });
        }, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
    }

    public void Dispose()
    {
        _refreshTimer?.Dispose();
    }

    protected async Task LoadDataAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            CheckIns = await CheckInAppService.GetLatestCheckInsAsync(50);
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading check-ins: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task RefreshData()
    {
        await LoadDataAsync();
    }
}
