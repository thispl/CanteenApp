using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.IO;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Branding settings service - Admin only
/// </summary>
[Authorize(Roles = "admin")]
public class BrandingSettingsAppService : ApplicationService, IBrandingSettingsAppService
{
    private const string CacheKey = "CMS_BrandingSettings";
    private const string LogoFolder = "wwwroot/images/branding";
    
    private readonly IDistributedCache _cache;

    public BrandingSettingsAppService(IDistributedCache cache)
    {
        _cache = cache;
    }

    [AllowAnonymous]
    public virtual async Task<BrandingSettingsDto> GetSettingsAsync()
    {
        var cached = await _cache.GetStringAsync(CacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<BrandingSettingsDto>(cached) ?? GetDefaults();
        }
        return GetDefaults();
    }

    public virtual async Task UpdateSettingsAsync(BrandingSettingsDto input)
    {
        var json = JsonSerializer.Serialize(input);
        await _cache.SetStringAsync(CacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365)
        });
    }

    public virtual async Task<string> UploadLogoAsync(byte[] fileData, string fileName)
    {
        if (fileData == null || fileData.Length == 0)
        {
            throw new UserFriendlyException("No file data provided");
        }

        if (fileData.Length > 2 * 1024 * 1024) // 2MB limit
        {
            throw new UserFriendlyException("File size exceeds 2MB limit");
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (extension != ".png" && extension != ".jpg" && extension != ".jpeg" && extension != ".svg")
        {
            throw new UserFriendlyException("Only PNG, JPG, JPEG, and SVG files are allowed");
        }

        var uploadsFolder = Path.Combine(Environment.CurrentDirectory, LogoFolder);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"logo_{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        await File.WriteAllBytesAsync(filePath, fileData);

        return $"/images/branding/{uniqueFileName}";
    }

    public virtual async Task ResetToDefaultsAsync()
    {
        var defaults = GetDefaults();
        await UpdateSettingsAsync(defaults);
    }

    private static BrandingSettingsDto GetDefaults()
    {
        return new BrandingSettingsDto
        {
            AppName = "CMS",
            LogoUrl = "/images/logo/leptonxlite/logo-light-thumbnail.png",
            LogoDarkUrl = "/images/logo/leptonxlite/logo-dark-thumbnail.png",
            FaviconUrl = "/favicon.ico",
            PrimaryColor = "#007bff",
            SecondaryColor = "#6c757d"
        };
    }
}
