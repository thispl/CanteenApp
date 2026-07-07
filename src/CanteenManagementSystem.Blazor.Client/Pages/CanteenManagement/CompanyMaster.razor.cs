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

public partial class CompanyMaster
{
    [Inject]
    protected ICompanyAppService CompanyAppService { get; set; } = null!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = null!;

    protected IReadOnlyList<CompanyDto> Companies { get; set; } = new List<CompanyDto>();
    protected long TotalCount { get; set; }

    protected CompanyListFilterDto Filter { get; set; } = new()
    {
        MaxResultCount = 20,
        SkipCount = 0
    };

    protected CreateCompanyDto NewCompany { get; set; } = new();
    protected UpdateCompanyDto EditingCompany { get; set; } = new();
    protected Guid EditingCompanyId { get; set; }

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
            var result = await CompanyAppService.GetListAsync(Filter);
            Companies = result.Items;
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading companies: {ex.Message}");
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
        NewCompany = new CreateCompanyDto();
        CreateModal.Show();
    }

    protected void CloseCreateModal()
    {
        CreateModal.Hide();
    }

    protected void OpenEditModal(CompanyDto company)
    {
        EditingCompanyId = company.Id;
        EditingCompany = new UpdateCompanyDto
        {
            Name = company.Name,
            Code = company.Code,
            Address = company.Address,
            Phone = company.Phone,
            Email = company.Email,
            TaxNumber = company.TaxNumber,
            Website = company.Website
        };
        EditModal.Show();
    }

    protected void CloseEditModal()
    {
        EditModal.Hide();
    }

    protected async Task CreateCompanyAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCompany.Name))
        {
            await UiMessageService.Error("Name is required");
            return;
        }

        try
        {
            await CompanyAppService.CreateAsync(NewCompany);
            await UiMessageService.Success("Company created successfully");
            CloseCreateModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error creating company: {ex.Message}");
        }
    }

    protected async Task UpdateCompanyAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingCompany.Name))
        {
            await UiMessageService.Error("Name is required");
            return;
        }

        try
        {
            await CompanyAppService.UpdateAsync(EditingCompanyId, EditingCompany);
            await UiMessageService.Success("Company updated successfully");
            CloseEditModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error updating company: {ex.Message}");
        }
    }

    protected async Task DeleteCompanyAsync(CompanyDto company)
    {
        var confirmed = await UiMessageService.Confirm(
            L["ConfirmDeleteCompany"],
            "Delete Confirmation");

        if (confirmed)
        {
            try
            {
                await CompanyAppService.DeleteAsync(company.Id);
                await UiMessageService.Success("Company deleted successfully");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await UiMessageService.Error($"Error deleting company: {ex.Message}");
            }
        }
    }
}
