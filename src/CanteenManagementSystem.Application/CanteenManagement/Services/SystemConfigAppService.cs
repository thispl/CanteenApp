using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

[Authorize(Roles = "admin")]
public class SystemConfigAppService : ApplicationService, ISystemConfigAppService
{
    private static string ApiSettingsPath =>
        Path.Combine(AppContext.BaseDirectory, "..", "Api", "appsettings.json");

    private static string BlazorSettingsPath =>
        Path.Combine(AppContext.BaseDirectory, "..", "Blazor", "appsettings.json");

    [AllowAnonymous]
    public virtual async Task<SystemConfigDto> GetConfigAsync()
    {
        var dto = new SystemConfigDto();
        try
        {
            var apiPath = ResolveApiPath();
            if (File.Exists(apiPath))
            {
                var json = await File.ReadAllTextAsync(apiPath);
                var node = JsonNode.Parse(json)!;
                dto.ApiUrl = node["App"]?["SelfUrl"]?.GetValue<string>() ?? dto.ApiUrl;
                dto.DefaultConnectionString = node["ConnectionStrings"]?["Default"]?.GetValue<string>() ?? string.Empty;
                dto.ZkTecoConnectionString = node["ConnectionStrings"]?["ZkTeco"]?.GetValue<string>() ?? string.Empty;
                dto.CorsOrigins = node["App"]?["CorsOrigins"]?.GetValue<string>() ?? string.Empty;
            }

            var blazorPath = ResolveBlazorPath();
            if (File.Exists(blazorPath))
            {
                var json = await File.ReadAllTextAsync(blazorPath);
                var node = JsonNode.Parse(json)!;
                dto.BlazorUrl = node["Kestrel"]?["Endpoints"]?["Http"]?["Url"]?.GetValue<string>() ?? dto.BlazorUrl;
            }
        }
        catch
        {
            // Return defaults if files not accessible
        }
        return dto;
    }

    public virtual async Task<SystemConfigResultDto> SaveConfigAsync(SystemConfigDto input)
    {
        try
        {
            // Update API appsettings.json
            var apiPath = ResolveApiPath();
            if (File.Exists(apiPath))
            {
                var json = await File.ReadAllTextAsync(apiPath);
                var node = JsonNode.Parse(json)!;

                node["App"]!["SelfUrl"] = input.ApiUrl;
                node["App"]!["CorsOrigins"] = input.BlazorUrl + ",";
                node["App"]!["RedirectAllowedUrls"] = input.BlazorUrl + ",";
                node["Kestrel"]!["Endpoints"]!["Http"]!["Url"] = input.ApiUrl;
                node["AuthServer"]!["Authority"] = input.ApiUrl;
                node["ConnectionStrings"]!["Default"] = input.DefaultConnectionString;
                node["ConnectionStrings"]!["ZkTeco"] = input.ZkTecoConnectionString;

                var options = new JsonSerializerOptions { WriteIndented = true };
                await File.WriteAllTextAsync(apiPath, node.ToJsonString(options));
            }

            // Update Blazor appsettings.json
            var blazorPath = ResolveBlazorPath();
            if (File.Exists(blazorPath))
            {
                var json = await File.ReadAllTextAsync(blazorPath);
                var node = JsonNode.Parse(json)!;
                node["Kestrel"]!["Endpoints"]!["Http"]!["Url"] = input.BlazorUrl;
                var options = new JsonSerializerOptions { WriteIndented = true };
                await File.WriteAllTextAsync(blazorPath, node.ToJsonString(options));
            }

            return new SystemConfigResultDto
            {
                Success = true,
                Message = "Configuration saved. Restart the service for changes to take effect.",
                RestartRequired = true
            };
        }
        catch (Exception ex)
        {
            return new SystemConfigResultDto
            {
                Success = false,
                Message = $"Failed to save: {ex.Message}"
            };
        }
    }

    public virtual async Task<SystemConfigResultDto> TestConnectionAsync(string connectionString)
    {
        try
        {
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            return new SystemConfigResultDto { Success = true, Message = "Connection successful!" };
        }
        catch (Exception ex)
        {
            return new SystemConfigResultDto { Success = false, Message = ex.Message };
        }
    }

    private static string ResolveApiPath()
    {
        // When running as service from ServiceHost, layout is d:\CanteenServices\ServiceHost\
        // API is at d:\CanteenServices\Api\appsettings.json
        var serviceHostDir = AppContext.BaseDirectory;
        var parent = Directory.GetParent(serviceHostDir)?.FullName ?? serviceHostDir;
        return Path.Combine(parent, "Api", "appsettings.json");
    }

    private static string ResolveBlazorPath()
    {
        var serviceHostDir = AppContext.BaseDirectory;
        var parent = Directory.GetParent(serviceHostDir)?.FullName ?? serviceHostDir;
        return Path.Combine(parent, "Blazor", "appsettings.json");
    }
}
