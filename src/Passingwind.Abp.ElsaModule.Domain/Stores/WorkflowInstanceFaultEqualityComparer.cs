using System;
using System.Collections.Generic;
using Passingwind.Abp.ElsaModule.Common;

namespace Passingwind.Abp.ElsaModule.Stores;

/// <summary>
///  TODO 
/// </summary>
public class WorkflowInstanceFaultEqualityComparer : IEqualityComparer<WorkflowInstanceFault>
{
    public static WorkflowInstanceFaultEqualityComparer Instance => new();

    public bool Equals(WorkflowInstanceFault x, WorkflowInstanceFault y)
    {
        return x != null && y != null
            && x.FaultedActivityId == y.FaultedActivityId
            ;
    }

    public int GetHashCode(WorkflowInstanceFault obj)
    {
        return HashCode.Combine(obj.FaultedActivityId);
    }
}
