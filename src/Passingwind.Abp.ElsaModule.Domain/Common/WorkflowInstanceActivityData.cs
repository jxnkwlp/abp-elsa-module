using System;
using System.Collections.Generic;
using Entity = Volo.Abp.Domain.Entities.Entity;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowInstanceActivityData : Entity
{
    public Guid WorkflowInstanceId { get; set; }
    public Guid ActivityId { get; set; }
    public Dictionary<string, object> Data { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { WorkflowInstanceId, ActivityId };
    }
}
