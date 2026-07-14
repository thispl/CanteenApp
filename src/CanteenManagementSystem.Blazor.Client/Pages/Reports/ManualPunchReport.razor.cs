using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Reports;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Components.Messages;

namespace CanteenManagementSystem.Blazor.Client.Pages.Reports;

public partial class ManualPunchReport
{
    [Inject] protected IReportAppService ReportAppService { get; set; } = null!;
    [Inject] protected IUiMessageService UiMessageService { get; set; } = null!;
    [Inject] protected IJSRuntime JS { get; set; } = null!;

    protected MonthlyReportFilterDto Filter { get; set; } = new();
    protected List<ManualPunchReportRowDto> Rows { get; set; } = new();
    protected bool IsLoading { get; set; }

    protected override async Task OnInitializedAsync() => await LoadDataAsync();

    protected async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();
            Rows = await ReportAppService.GetManualPunchReportAsync(Filter);
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
        var url = $"/api/app/reports/excel/manual-punch?from={Filter.From:yyyy-MM-dd}&to={Filter.To:yyyy-MM-dd}";
        await JS.InvokeVoidAsync("open", url, "_blank");
    }

    protected void ExportCsv()
    {
        var lines = new System.Text.StringBuilder();
        lines.AppendLine("Employee ID,Name,Department,Punch Time,Schedule,Item,Price");
        foreach (var r in Rows)
            lines.AppendLine($"\"{r.EmployeeIdNumber}\",\"{r.EmployeeName}\",\"{r.Department}\",\"{r.PunchTime:yyyy-MM-dd HH:mm}\",\"{r.TimeScheduleName}\",\"{r.ItemDescription}\",{r.Price}");
        lines.AppendLine($"TOTAL,,,,,,{Rows.Sum(r => r.Price)}");
        DownloadCsv(lines.ToString(), $"manual-punch-{Filter.From:yyyyMM}.csv");
    }

    private async void DownloadCsv(string content, string filename)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var base64 = Convert.ToBase64String(bytes);
        await JS.InvokeVoidAsync("eval",
            $"var a=document.createElement('a');a.href='data:text/csv;base64,{base64}';a.download='{filename}';a.click();");
    }
}
