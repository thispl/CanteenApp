using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// Company master representing the organizations in the system.
/// </summary>
public class Company : FullAuditedEntity<Guid>
{
    /// <summary>
    /// Company name
    /// </summary>
    public virtual string Name { get; protected set; }

    /// <summary>
    /// Optional short code
    /// </summary>
    public virtual string? Code { get; protected set; }

    /// <summary>
    /// Company address
    /// </summary>
    public virtual string? Address { get; protected set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public virtual string? Phone { get; protected set; }

    /// <summary>
    /// Email address
    /// </summary>
    public virtual string? Email { get; protected set; }

    /// <summary>
    /// Tax / GST number
    /// </summary>
    public virtual string? TaxNumber { get; protected set; }

    /// <summary>
    /// Website URL
    /// </summary>
    public virtual string? Website { get; protected set; }

    protected Company()
    {
        // Required by EF Core
    }

    public Company(
        Guid id,
        string name,
        string? code = null,
        string? address = null,
        string? phone = null,
        string? email = null,
        string? taxNumber = null,
        string? website = null)
    {
        Id = id;
        Name = name;
        Code = code;
        Address = address;
        Phone = phone;
        Email = email;
        TaxNumber = taxNumber;
        Website = website;
    }

    public virtual void SetName(string name)
    {
        Name = name;
    }

    public virtual void SetCode(string? code)
    {
        Code = code;
    }

    public virtual void SetAddress(string? address)
    {
        Address = address;
    }

    public virtual void SetPhone(string? phone)
    {
        Phone = phone;
    }

    public virtual void SetEmail(string? email)
    {
        Email = email;
    }

    public virtual void SetTaxNumber(string? taxNumber)
    {
        TaxNumber = taxNumber;
    }

    public virtual void SetWebsite(string? website)
    {
        Website = website;
    }
}
