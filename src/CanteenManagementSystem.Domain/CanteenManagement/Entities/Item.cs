using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// Item master representing canteen menu items and their prices.
/// </summary>
public class Item : FullAuditedEntity<Guid>
{
    /// <summary>
    /// Description of the item
    /// </summary>
    public virtual string Description { get; protected set; }

    /// <summary>
    /// Price of the item (must be >= 0)
    /// </summary>
    public virtual decimal Price { get; protected set; }

    protected Item()
    {
        // Required by EF Core
    }

    public Item(
        Guid id,
        string description,
        decimal price)
    {
        Id = id;
        Description = description;
        Price = price;
    }

    public virtual void SetDescription(string description)
    {
        Description = description;
    }

    public virtual void SetPrice(decimal price)
    {
        Price = price;
    }
}
