using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;

namespace CanteenManagementSystem.Blazor.Client.Pages.Reports;

/// <summary>
/// Shared helper for downloading Excel/CSV files from report pages.
/// Excel: fetches bytes from the API with Bearer auth, triggers browser download via JS Blob.
/// CSV:   generates content in-memory and triggers browser download via JS Blob.
/// </summary>
public static class ReportExportHelper
{
    private const string ApiBase = "http://localhost:5002";
    private const string ExcelMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string CsvMime = "text/csv";

    /// <summary>
    /// Downloads an Excel file from the report API using the current OIDC access token.
    /// Uses window.downloadFileFromBytes (defined in branding.js) to trigger the save dialog.
    /// </summary>
    public static async Task DownloadExcelAsync(
        IJSRuntime js,
        IAccessTokenProvider tokenProvider,
        string reportKey,
        string queryString,
        string filename)
    {
        // 1. Try to get a cached access token (non-interactive)
        var tokenResult = await tokenProvider.RequestAccessToken();

        string? accessToken = null;
        if (tokenResult.TryGetToken(out var token))
        {
            accessToken = token.Value;
        }
        else
        {
            // Token needs a refresh — try with an explicit options object that retries
            // If that also fails the user must re-login
            var options = new AccessTokenRequestOptions { Scopes = new[] { "CanteenManagementSystem" } };
            tokenResult = await tokenProvider.RequestAccessToken(options);
            if (tokenResult.TryGetToken(out token))
            {
                accessToken = token.Value;
            }
        }

        if (string.IsNullOrEmpty(accessToken))
        {
            await js.InvokeVoidAsync("alert",
                "Could not obtain access token.\n" +
                "Please refresh the page (F5) and try again.\n\n" +
                $"Token status: {tokenResult.Status}");
            return;
        }

        // 2. Fetch Excel bytes from the API with Bearer auth
        using var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var url = $"{ApiBase}/api/app/reports/excel/{reportKey}?{queryString}";

        HttpResponseMessage response;
        try
        {
            response = await http.GetAsync(url);
        }
        catch (Exception ex)
        {
            await js.InvokeVoidAsync("alert", $"Network error during export:\n{ex.Message}");
            return;
        }

        // 3. Verify we got a real Excel file back (not a login HTML page)
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
        if (!response.IsSuccessStatusCode || !contentType.Contains("spreadsheetml"))
        {
            var body = await response.Content.ReadAsStringAsync();
            // Strip HTML tags for a readable error
            var clean = System.Text.RegularExpressions.Regex.Replace(body, "<[^>]+>", "").Trim();
            var preview = clean.Length > 400 ? clean[..400] + "…" : clean;
            await js.InvokeVoidAsync("alert",
                $"Export failed (HTTP {(int)response.StatusCode})\n" +
                $"Content-Type: {contentType}\n\n" +
                $"{preview}");
            return;
        }

        // 4. Trigger browser save-dialog — passes base64 as a JS argument (safe, no string interpolation)
        var bytes = await response.Content.ReadAsByteArrayAsync();
        var base64 = Convert.ToBase64String(bytes);
        await js.InvokeVoidAsync("downloadFileFromBytes", filename, ExcelMime, base64);
    }

    /// <summary>
    /// Triggers a CSV download entirely client-side using the JS Blob helper.
    /// </summary>
    public static async Task DownloadCsvAsync(IJSRuntime js, string csvContent, string filename)
    {
        var bytes = Encoding.UTF8.GetBytes(csvContent);
        var base64 = Convert.ToBase64String(bytes);
        await js.InvokeVoidAsync("downloadFileFromBytes", filename, CsvMime, base64);
    }
}
