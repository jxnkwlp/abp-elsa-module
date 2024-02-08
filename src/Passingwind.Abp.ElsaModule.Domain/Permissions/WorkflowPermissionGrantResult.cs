using System;
using System.Collections.Generic;
using System.Linq;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class WorkflowPermissionGrantResult
{
    public bool AllGranted => WorkflowGrantProviders.Keys.Count == 0;
    public IEnumerable<Guid> WorkflowIds => WorkflowGrantProviders.Keys;
    public Dictionary<Guid, IReadOnlyList<WorkflowPermissionGrantProvider>> WorkflowGrantProviders { get; protected set; }

    protected WorkflowPermissionGrantResult()
    {
        WorkflowGrantProviders = new Dictionary<Guid, IReadOnlyList<WorkflowPermissionGrantProvider>>();
    }

    public static WorkflowPermissionGrantResult Empty => new WorkflowPermissionGrantResult();

    public static WorkflowPermissionGrantResult Create() => new WorkflowPermissionGrantResult();

    public WorkflowPermissionGrantResult AddWorkflow(Guid workflowId, params WorkflowPermissionGrantProvider[] provider)
    {
        WorkflowGrantProviders[workflowId] = provider;

        return this;
    }

    public WorkflowPermissionGrantResult AddProvider(Guid workflowId, string name, string value)
    {
        lock (WorkflowGrantProviders)
        {
            if (WorkflowGrantProviders.TryGetValue(workflowId, out var grantProviders))
            {
                var providers = grantProviders.ToList();

                providers.Add(new WorkflowPermissionGrantProvider(name, value));
                WorkflowGrantProviders[workflowId] = providers;
            }
            else
            {
                WorkflowGrantProviders[workflowId] = new List<WorkflowPermissionGrantProvider> { new WorkflowPermissionGrantProvider(name, value) };
            }
        }

        return this;
    }
}
