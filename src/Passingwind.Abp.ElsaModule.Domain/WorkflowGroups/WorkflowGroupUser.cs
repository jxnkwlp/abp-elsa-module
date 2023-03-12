using System;
using Volo.Abp.Domain.Entities;

namespace Passingwind.Abp.ElsaModule.WorkflowGroups;

public class WorkflowGroupUser : Entity
{
    public Guid WorkflowGroupId { get; set; }
    public Guid UserId { get; set; }

    public WorkflowGroupUser(Guid userId)
    {
        UserId = userId;
    }

    public override object[] GetKeys()
    {
        return new object[] { WorkflowGroupId, UserId };
    }
}
