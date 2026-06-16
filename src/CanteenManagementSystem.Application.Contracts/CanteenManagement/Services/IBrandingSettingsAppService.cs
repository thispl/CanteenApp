using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;

namespace CanteenManagementSystem.CanteenManagement.Services;

/// <summary>
/// Application service for managing branding settings (Admin only)
/// </summary>
public interface IBrandingSettingsAppService : IApplicationService
{
    /// <summary>
    /// Get current branding settings
    /// </summary>
    Task<BrandingSettingsDto> GetSettingsAsync();

    /// <summary>
    /// Update branding settings (Admin only)
    /// </summary>
    Task UpdateSettingsAsync(BrandingSettingsDto input);

    /// <summary>
    /// Upload logo file
    /// </summary>
    Task<string> UploadLogoAsync(byte[] fileData, string fileName);

    /// <summary>
    /// Reset to default branding
    /// </summary>
    Task ResetToDefaultsAsync();
}
