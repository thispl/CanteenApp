using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Reports;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Messages;

namespace CanteenManagementSystem.Blazor.Client.Pages.Reports;

public partial class DailySummaryReport
{
    [Inject] protected IReportAppService ReportAppService { get; set; } = null!;
    [Inject] protected IUiMessageService UiMessageService { get; set; } = null!;
    [Inject] protected IJSRuntime JS { get; set; } = null!;

    protected DailyReportFilterDto Filter { get; set; } = new();
    protected List<DailySummaryReportRowDto> Rows { get; set; } = new();
    protected bool IsLoading { get; set; }

    protected override async Task OnInitializedAsync() => await LoadDataAsync();

    protected async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();
            Rows = await ReportAppService.GetDailySummaryAsync(Filter);
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
        var url = $"/api/app/reports/excel/daily-summary?date={Filter.Date:yyyy-MM-dd}";
        await JS.InvokeVoidAsync("open", url, "_blank");
    }

    protected void ExportCsv()
    {
        var lines = new System.Text.StringBuilder();
        lines.AppendLine("Time Schedule,Slot,Count,Total Amount");
        foreach (var r in Rows)
            lines.AppendLine($"\"{r.TimeScheduleName}\",\"{r.TimeSlot}\",{r.TransactionCount},{r.TotalAmount}");
        lines.AppendLine($"TOTAL,,,{Rows.Sum(r => r.TransactionCount)},{Rows.Sum(r => r.TotalAmount)}");
        DownloadCsv(lines.ToString(), $"daily-summary-{Filter.Date:yyyyMMdd}.csv");
    }

    private async void DownloadCsv(string content, string filename)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var base64 = Convert.ToBase64String(bytes);
        await JS.InvokeVoidAsync("eval",
            $"var a=document.createElement('a');a.href='data:text/csv;base64,{base64}';a.download='{filename}';a.click();");
    }
}
