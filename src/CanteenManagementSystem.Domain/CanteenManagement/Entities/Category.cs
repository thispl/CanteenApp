using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// Category master for classifying employees/groups in the canteen system.
/// </summary>
public class Category : FullAuditedEntity<Guid>
{
    /// <summary>
    /// Name of the category
    /// </summary>
    public virtual string CategoryName { get; protected set; }

    /// <summary>
    /// Optional unique code for the category
    /// </summary>
    public virtual string? CategoryCode { get; protected set; }

    protected Category()
    {
        // Required by EF Core
    }

    public Category(
        Guid id,
        string categoryName,
        string? categoryCode = null)
    {
        Id = id;
        CategoryName = categoryName;
        CategoryCode = categoryCode;
    }

    public virtual void SetCategoryName(string categoryName)
    {
        CategoryName = categoryName;
    }

    public virtual void SetCategoryCode(string? categoryCode)
    {
        CategoryCode = categoryCode;
    }
}
