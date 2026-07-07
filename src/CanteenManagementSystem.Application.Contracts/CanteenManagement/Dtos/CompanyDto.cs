using System;
using Volo.Abp.Application.Dtos;

namespace CanteenManagementSystem.CanteenManagement.Dtos;

/// <summary>
/// DTO for Company entity
/// </summary>
public class CompanyDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxNumber { get; set; }
    public string? Website { get; set; }
}

/// <summary>
/// DTO for creating a new company
/// </summary>
public class CreateCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxNumber { get; set; }
    public string? Website { get; set; }
}

/// <summary>
/// DTO for updating a company
/// </summary>
public class UpdateCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxNumber { get; set; }
    public string? Website { get; set; }
}

/// <summary>
/// DTO for filtering companies
/// </summary>
public class CompanyListFilterDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
