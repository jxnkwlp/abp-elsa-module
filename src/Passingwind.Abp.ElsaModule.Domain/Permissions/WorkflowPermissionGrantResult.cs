using System;
using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowPermissionGrantResult
{
    public bool AllGranted { get; private set; }
    public IEnumerable<Guid> WorkflowIds => WorkflowRoleMaps.Keys;
    public Dictionary<Guid, IEnumerable<WorkflowPermissionGrantProvider>> WorkflowRoleMaps { get; set; }

    protected WorkflowPermissionGrantResult()
    {
        WorkflowRoleMaps = new Dictionary<Guid, IEnumerable<WorkflowPermissionGrantProvider>>();
    }

    public WorkflowPermissionGrantResult(Dictionary<Guid, IEnumerable<WorkflowPermissionGrantProvider>> maps)
    {
        WorkflowRoleMaps = maps;
    }

    public static WorkflowPermissionGrantResult All => new WorkflowPermissionGrantResult() { AllGranted = true };
    public static WorkflowPermissionGrantResult Empty => new WorkflowPermissionGrantResult();
}
