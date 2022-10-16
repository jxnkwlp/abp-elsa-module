using System;
using Entity = Volo.Abp.Domain.Entities.Entity;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowInstanceBlockingActivity : Entity
{
    public Guid WorkflowInstanceId { get; set; }
    public Guid ActivityId { get; set; }
    public string ActivityType { get; set; }
    public string Tag { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { WorkflowInstanceId, ActivityId };
    }
}
