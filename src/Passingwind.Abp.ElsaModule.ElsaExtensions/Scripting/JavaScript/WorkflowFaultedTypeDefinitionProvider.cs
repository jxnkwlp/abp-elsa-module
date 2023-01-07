using System;
using System.Collections.Generic;
using Elsa.Scripting.JavaScript.Services;
using Passingwind.Abp.ElsaModule.Activities.Workflows;

namespace Passingwind.Abp.ElsaModule.Scripting.JavaScript;

public class WorkflowFaultedTypeDefinitionProvider : TypeDefinitionProvider
{
    public override IEnumerable<Type> CollectTypes(TypeDefinitionContext context)
    {
        yield return typeof(WorkflowFaultedInput);
    }
}
