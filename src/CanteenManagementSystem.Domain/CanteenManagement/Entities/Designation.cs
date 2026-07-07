using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// Designation master representing employee job titles.
/// </summary>
public class Designation : FullAuditedEntity<Guid>
{
    /// <summary>
    /// Title of the designation (e.g., Manager, Engineer)
    /// </summary>
    public virtual string Title { get; protected set; }

    /// <summary>
    /// Optional short code
    /// </summary>
    public virtual string? Code { get; protected set; }

    /// <summary>
    /// Optional description
    /// </summary>
    public virtual string? Description { get; protected set; }

    protected Designation()
    {
        // Required by EF Core
    }

    public Designation(
        Guid id,
        string title,
        string? code = null,
        string? description = null)
    {
        Id = id;
        Title = title;
        Code = code;
        Description = description;
    }

    public virtual void SetTitle(string title)
    {
        Title = title;
    }

    public virtual void SetCode(string? code)
    {
        Code = code;
    }

    public virtual void SetDescription(string? description)
    {
        Description = description;
    }
}
