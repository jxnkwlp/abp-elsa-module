using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

/// <summary>
///  The global code content
/// </summary>
public class GlobalCodeContent : BasicAggregateRoot<Guid>, IMultiTenant
{
    protected GlobalCodeContent()
    {
    }

    public GlobalCodeContent(Guid id, Guid globalCodeId, int version, string content, Guid? tenantId = null) : base(id)
    {
        GlobalCodeId = globalCodeId;
        Version = version;
        Content = content;
        TenantId = tenantId;
    }

    public Guid GlobalCodeId { get; protected set; }

    public int Version { get; set; }

    public string Content { get; set; }

    public Guid? TenantId { get; protected set; }
}
