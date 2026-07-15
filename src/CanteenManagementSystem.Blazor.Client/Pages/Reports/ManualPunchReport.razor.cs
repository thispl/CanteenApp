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

public partial class ManualPunchReport
{
    [Inject] protected IReportAppService ReportAppService { get; set; } = null!;
    [Inject] protected IUiMessageService UiMessageService { get; set; } = null!;
    [Inject] protected IJSRuntime JS { get; set; } = null!;
    [Inject] protected IAccessTokenProvider TokenProvider { get; set; } = null!;

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
        await ReportExportHelper.DownloadExcelAsync(JS, TokenProvider,
            "manual-punch",
            $"from={Filter.From:yyyy-MM-dd}&to={Filter.To:yyyy-MM-dd}",
            $"manual-punch-{Filter.From:yyyyMM}.xlsx");
    }

    protected async Task ExportCsv()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Employee ID,Name,Department,Punch Time,Schedule,Item,Price");
        foreach (var r in Rows)
            sb.AppendLine($"\"{r.EmployeeIdNumber}\",\"{r.EmployeeName}\",\"{r.Department}\",\"{r.PunchTime:yyyy-MM-dd HH:mm}\",\"{r.TimeScheduleName}\",\"{r.ItemDescription}\",{r.Price}");
        sb.AppendLine($"TOTAL,,,,,,{Rows.Sum(r => r.Price)}");
        await ReportExportHelper.DownloadCsvAsync(JS, sb.ToString(), $"manual-punch-{Filter.From:yyyyMM}.csv");
    }
}
