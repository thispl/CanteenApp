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

public partial class DesignationMaster
{
    [Inject]
    protected IDesignationAppService DesignationAppService { get; set; } = null!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = null!;

    protected IReadOnlyList<DesignationDto> Designations { get; set; } = new List<DesignationDto>();
    protected long TotalCount { get; set; }

    protected DesignationListFilterDto Filter { get; set; } = new()
    {
        MaxResultCount = 20,
        SkipCount = 0
    };

    protected CreateDesignationDto NewDesignation { get; set; } = new();
    protected UpdateDesignationDto EditingDesignation { get; set; } = new();
    protected Guid EditingDesignationId { get; set; }

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
            var result = await DesignationAppService.GetListAsync(Filter);
            Designations = result.Items;
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading designations: {ex.Message}");
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
        NewDesignation = new CreateDesignationDto();
        CreateModal.Show();
    }

    protected void CloseCreateModal()
    {
        CreateModal.Hide();
    }

    protected void OpenEditModal(DesignationDto designation)
    {
        EditingDesignationId = designation.Id;
        EditingDesignation = new UpdateDesignationDto
        {
            Title = designation.Title,
            Code = designation.Code,
            Description = designation.Description
        };
        EditModal.Show();
    }

    protected void CloseEditModal()
    {
        EditModal.Hide();
    }

    protected async Task CreateDesignationAsync()
    {
        if (string.IsNullOrWhiteSpace(NewDesignation.Title))
        {
            await UiMessageService.Error("Title is required");
            return;
        }

        try
        {
            await DesignationAppService.CreateAsync(NewDesignation);
            await UiMessageService.Success("Designation created successfully");
            CloseCreateModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error creating designation: {ex.Message}");
        }
    }

    protected async Task UpdateDesignationAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingDesignation.Title))
        {
            await UiMessageService.Error("Title is required");
            return;
        }

        try
        {
            await DesignationAppService.UpdateAsync(EditingDesignationId, EditingDesignation);
            await UiMessageService.Success("Designation updated successfully");
            CloseEditModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error updating designation: {ex.Message}");
        }
    }

    protected async Task DeleteDesignationAsync(DesignationDto designation)
    {
        var confirmed = await UiMessageService.Confirm(
            L["ConfirmDeleteDesignation"],
            "Delete Confirmation");

        if (confirmed)
        {
            try
            {
                await DesignationAppService.DeleteAsync(designation.Id);
                await UiMessageService.Success("Designation deleted successfully");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error($"Error deleting designation: {ex.Message}");
            }
        }
    }
}
