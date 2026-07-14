using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CanteenManagementSystem.CanteenManagement.Entities;

/// <summary>
/// Department master representing organizational units/cost centers.
/// </summary>
public class Department : FullAuditedEntity<Guid>
{
    /// <summary>
    /// Name of the department
    /// </summary>
    public virtual string Name { get; protected set; }

    /// <summary>
    /// Optional cost-center code
    /// </summary>
    public virtual string? CCCode { get; protected set; }

    /// <summary>
    /// Company this department belongs to (optional)
    /// </summary>
    public virtual Guid? CompanyId { get; protected set; }
    public virtual Company? Company { get; protected set; }

    protected Department()
    {
        // Required by EF Core
    }

    public Department(
        Guid id,
        string name,
        string? ccCode = null,
        Guid? companyId = null)
    {
        Id = id;
        Name = name;
        CCCode = ccCode;
        CompanyId = companyId;
    }

    public virtual void SetName(string name)
    {
        Name = name;
    }

    public virtual void SetCCCode(string? ccCode)
    {
        CCCode = ccCode;
    }

    public virtual void SetCompany(Guid? companyId)
    {
        CompanyId = companyId;
    }
}
