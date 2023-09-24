using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

/// <summary>
///  The global code version history
/// </summary>
public class GlobalCodeVersion : AuditedAggregateRoot<Guid>, IMultiTenant
{
    protected GlobalCodeVersion()
    {
    }

    public GlobalCodeVersion(Guid id, Guid globalCodeId, int version, Guid? tenantId = null) : base(id)
    {
        GlobalCodeId = globalCodeId;
        Version = version;
        TenantId = tenantId;
    }

    public Guid GlobalCodeId { get; set; }
    public int Version { get; set; }
    public Guid? TenantId { get; protected set; }
}
