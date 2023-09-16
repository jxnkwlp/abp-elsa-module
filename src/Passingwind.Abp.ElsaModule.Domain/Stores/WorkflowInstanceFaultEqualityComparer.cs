using System;
using System.Collections.Generic;
using Passingwind.Abp.ElsaModule.Common;

namespace Passingwind.Abp.ElsaModule.Stores;

/// <summary>
///  TODO
/// </summary>
public class WorkflowInstanceFaultEqualityComparer : IEqualityComparer<WorkflowInstanceFault>, System.Collections.IEqualityComparer
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

    new public bool Equals(object x, object y)
    {
        if (x == y)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        if (x is WorkflowInstanceFault a
            && y is WorkflowInstanceFault b)
        {
            return Equals(a, b);
        }

        throw new ArgumentException("", nameof(x));
    }

    public int GetHashCode(object obj)
    {
        if (obj == null)
        {
            return 0;
        }

        if (obj is WorkflowInstanceFault x)
        {
            return GetHashCode(x);
        }

        throw new ArgumentException("", nameof(obj));
    }
}
