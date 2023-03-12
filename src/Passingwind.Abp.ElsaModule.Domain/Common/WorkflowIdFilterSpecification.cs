using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Volo.Abp.Specifications;

namespace Passingwind.Abp.ElsaModule.Common;

public class WorkflowIdFilterSpecification : Specification<WorkflowDefinition>
{
    public IEnumerable<Guid> Ids { get; }

    public WorkflowIdFilterSpecification(IEnumerable<Guid> ids)
    {
        Ids = ids;
    }

    public override Expression<Func<WorkflowDefinition, bool>> ToExpression()
    {
        if (Ids?.Any() == true)
            return x => Ids.Contains(x.Id);
        return x => true;
    }
}
