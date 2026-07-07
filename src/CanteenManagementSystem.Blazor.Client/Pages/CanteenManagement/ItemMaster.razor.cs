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

public partial class ItemMaster
{
    [Inject]
    protected IItemAppService ItemAppService { get; set; } = null!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = null!;

    protected IReadOnlyList<ItemDto> Items { get; set; } = new List<ItemDto>();
    protected long TotalCount { get; set; }

    protected ItemListFilterDto Filter { get; set; } = new()
    {
        MaxResultCount = 20,
        SkipCount = 0
    };

    protected CreateItemDto NewItem { get; set; } = new();
    protected UpdateItemDto EditingItem { get; set; } = new();
    protected Guid EditingItemId { get; set; }

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
            var result = await ItemAppService.GetListAsync(Filter);
            Items = result.Items;
            TotalCount = result.TotalCount;
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
        NewItem = new CreateItemDto();
        CreateModal.Show();
    }

    protected void CloseCreateModal()
    {
        CreateModal.Hide();
    }

    protected void OpenEditModal(ItemDto item)
    {
        EditingItemId = item.Id;
        EditingItem = new UpdateItemDto
        {
            Description = item.Description,
            Price = item.Price
        };
        EditModal.Show();
    }

    protected void CloseEditModal()
    {
        EditModal.Hide();
    }

    protected async Task CreateItemAsync()
    {
        if (string.IsNullOrWhiteSpace(NewItem.Description))
        {
            await UiMessageService.Error("Description is required");
            return;
        }

        try
        {
            await ItemAppService.CreateAsync(NewItem);
            await UiMessageService.Success("Item created successfully");
            CloseCreateModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error creating item: {ex.Message}");
        }
    }

    protected async Task UpdateItemAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingItem.Description))
        {
            await UiMessageService.Error("Description is required");
            return;
        }

        try
        {
            await ItemAppService.UpdateAsync(EditingItemId, EditingItem);
            await UiMessageService.Success("Item updated successfully");
            CloseEditModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error updating item: {ex.Message}");
        }
    }

    protected async Task DeleteItemAsync(ItemDto item)
    {
        var confirmed = await UiMessageService.Confirm(
            L["ConfirmDeleteItem"],
            "Delete Confirmation");

        if (confirmed)
        {
            try
            {
                await ItemAppService.DeleteAsync(item.Id);
                await UiMessageService.Success("Item deleted successfully");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error($"Error deleting item: {ex.Message}");
            }
        }
    }
}
