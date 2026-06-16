using System.ComponentModel.DataAnnotations;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

/// <summary>
/// Branding settings DTO for admin configuration
/// </summary>
public class BrandingSettingsDto
{
    /// <summary>
    /// Application name/display title
    /// </summary>
    [Required]
    [StringLength(50)]
    public string AppName { get; set; } = "CMS";

    /// <summary>
    /// Logo URL or base64 data
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Dark logo for light backgrounds
    /// </summary>
    public string? LogoDarkUrl { get; set; }

    /// <summary>
    /// Favicon URL
    /// </summary>
    public string? FaviconUrl { get; set; }

    /// <summary>
    /// Primary theme color (hex)
    /// </summary>
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Must be a valid hex color like #007bff")]
    public string PrimaryColor { get; set; } = "#007bff";

    /// <summary>
    /// Secondary theme color (hex)
    /// </summary>
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Must be a valid hex color like #6c757d")]
    public string SecondaryColor { get; set; } = "#6c757d";
}
