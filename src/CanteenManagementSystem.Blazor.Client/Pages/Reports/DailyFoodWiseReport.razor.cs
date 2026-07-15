using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Reports;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Messages;

namespace CanteenManagementSystem.Blazor.Client.Pages.Reports;

public partial class DailyFoodWiseReport
{
    [Inject] protected IReportAppService ReportAppService { get; set; } = null!;
    [Inject] protected IUiMessageService UiMessageService { get; set; } = null!;
    [Inject] protected IJSRuntime JS { get; set; } = null!;
    [Inject] protected IAccessTokenProvider TokenProvider { get; set; } = null!;

    protected DailyReportFilterDto Filter { get; set; } = new();
    protected List<FoodWiseReportRowDto> Rows { get; set; } = new();
    protected bool IsLoading { get; set; }

    protected override async Task OnInitializedAsync() => await LoadDataAsync();

    protected async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();
            Rows = await ReportAppService.GetDailyFoodWiseAsync(Filter);
        }
        catch (Exception ex)
        {
            await UiMessageService.Error($"Error loading report: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task ExportExcel()
    {
        await ReportExportHelper.DownloadExcelAsync(JS, TokenProvider,
            "daily-food-wise",
            $"date={Filter.Date:yyyy-MM-dd}",
            $"daily-food-wise-{Filter.Date:yyyyMMdd}.xlsx");
    }

    protected async Task ExportCsv()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Item,Unit Price,Count,Total Amount");
        foreach (var r in Rows)
            sb.AppendLine($"\"{r.ItemDescription}\",{r.UnitPrice},{r.TransactionCount},{r.TotalAmount}");
        sb.AppendLine($"TOTAL,,{Rows.Sum(r => r.TransactionCount)},{Rows.Sum(r => r.TotalAmount)}");
        await ReportExportHelper.DownloadCsvAsync(JS, sb.ToString(), $"daily-food-wise-{Filter.Date:yyyyMMdd}.csv");
    }
}
