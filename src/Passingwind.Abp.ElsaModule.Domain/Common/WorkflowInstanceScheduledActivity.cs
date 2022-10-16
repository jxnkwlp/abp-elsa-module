using System;
using Entity = Volo.Abp.Domain.Entities.Entity;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowInstanceScheduledActivity : Entity
{
    public Guid WorkflowInstanceId { get; set; }
    public Guid ActivityId { get; set; }
    public object Input { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { WorkflowInstanceId, ActivityId };
    }
}
