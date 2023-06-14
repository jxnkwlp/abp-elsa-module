using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

public class WorkflowVariableUpdateDto
{
    public Dictionary<string, object> Variables { get; set; }
}
