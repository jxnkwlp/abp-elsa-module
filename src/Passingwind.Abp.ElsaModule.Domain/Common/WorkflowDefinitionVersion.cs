using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowDefinitionVersion : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public WorkflowDefinitionVersion()
    {
        Activities = new List<Activity>();
        Connections = new List<ActivityConnection>();
    }

    public WorkflowDefinitionVersion(Guid id) : base(id)
    {
    }

    public WorkflowDefinitionVersion(Guid definitionId, Guid? tenantId, List<Activity> activities, List<ActivityConnection> connections)
    {
        DefinitionId = definitionId;
        TenantId = tenantId;
        Activities = activities ?? new List<Activity>();
        Connections = connections ?? new List<ActivityConnection>();
    }

    // public WorkflowDefinition Definition { get; set; }

    public Guid DefinitionId { get; protected set; }

    public Guid? TenantId { get; protected set; }

    public int Version { get; protected set; } = 1;

    public bool IsLatest { get; protected set; }

    public bool IsPublished { get; protected set; }

    public List<Activity> Activities { get; set; }

    public List<ActivityConnection> Connections { get; set; }

    public void SetVersion(int version)
    {
        Version = version;
    }

    public void SetIsLatest(bool value)
    {
        IsLatest = value;
    }

    public void SetIsPublished(bool value)
    {
        IsPublished = value;
    }
}
