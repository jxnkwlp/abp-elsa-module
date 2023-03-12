using System;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Permissions;

namespace Passingwind.Abp.ElsaModule;

internal static class WorkflowHelper
{
    public static string GenerateWorkflowPermissionKey(Guid guid)
    {
        return $"{ElsaModulePermissions.Workflow.Default}.{guid.ToString("d")}";
    }

    public static string GenerateWorkflowPermissionKey(WorkflowDefinition workflow)
    {
        return $"{ElsaModulePermissions.Workflow.Default}.{workflow.Id.ToString("d")}";
    }

    public static Guid ResolveWorkflowIdFromPermissionKey(string key)
    {
        return Guid.Parse(key.Substring(ElsaModulePermissions.Workflow.Default.Length + 1));
    }
}
