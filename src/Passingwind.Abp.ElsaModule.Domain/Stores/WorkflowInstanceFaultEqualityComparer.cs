using System;
using System.Collections.Generic;
using System.Text.Json;
using Passingwind.Abp.ElsaModule.Common;

namespace Passingwind.Abp.ElsaModule.Stores;

public class WorkflowInstanceFaultEqualityComparer : IEqualityComparer<WorkflowInstanceFault>
{
    public bool Equals(WorkflowInstanceFault x, WorkflowInstanceFault y)
    {
        return x != null && y != null
            && x.Exception == y.Exception
            && x.FaultedActivityId == y.FaultedActivityId
            && x.Message == y.Message
            && x.Resuming == y.Resuming
            && JsonSerializer.Serialize(x.ActivityInput) == JsonSerializer.Serialize(y.ActivityInput)
            ;
    }

    public int GetHashCode(WorkflowInstanceFault obj)
    {
        return HashCode.Combine(obj.WorkflowInstanceId, obj.FaultedActivityId, obj.Message, obj.Resuming, JsonSerializer.Serialize(obj.ActivityInput));
    }
}
