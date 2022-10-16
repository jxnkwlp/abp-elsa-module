using System;
using Entity = Volo.Abp.Domain.Entities.Entity;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowInstanceVariable : Entity
{
    public Guid WorkflowInstanceId { get; set; }
    public string Key { get; set; }
    public object Value { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { WorkflowInstanceId, Key };
    }
}
