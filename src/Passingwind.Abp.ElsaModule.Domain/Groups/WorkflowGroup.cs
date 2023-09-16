using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Passingwind.Abp.ElsaModule.Groups;

/// <summary>
///  The group of workflow
/// </summary>
public class WorkflowGroup : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string Name { get; protected set; }
    public string Description { get; set; }
    public Guid? TenantId { get; protected set; }

    protected WorkflowGroup()
    {
    }

    public WorkflowGroup(Guid id, string name, Guid? tenantId = null) : base(id)
    {
        Name = name;
        TenantId = tenantId;
    }

    public void SetName(string value)
    {
        if (value == Name)
            return;

        var oldName = Name;
        Name = value;

        AddLocalEvent(new WorkflowGroupNameChangedEvent(oldName, value, Id));
    }
}
