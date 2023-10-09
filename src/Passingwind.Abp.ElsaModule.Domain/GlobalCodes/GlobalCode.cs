using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

/// <summary>
///  The global code for workflow activity
/// </summary>
public class GlobalCode : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    protected GlobalCode()
    {
    }

    public GlobalCode(Guid id, string name, GlobalCodeType type, Guid? tenantId = null) : base(id)
    {
        Name = name;
        Type = type;
        TenantId = tenantId;
    }

    public string Name { get; protected set; }
    public string Description { get; set; }

    public GlobalCodeLanguage Language { get; set; }

    public GlobalCodeType Type { get; set; }

    public int LatestVersion { get; protected set; } = 1;

    public int PublishedVersion { get; protected set; }

    public Guid? TenantId { get; protected set; }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetLatestVersion(int version)
    {
        LatestVersion = version;
    }

    public void SetPublishedVersion(int version)
    {
        PublishedVersion = version;
    }
}
