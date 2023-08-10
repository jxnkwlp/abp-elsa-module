using System;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Permissions;

namespace Passingwind.Abp.ElsaModule;

internal static class WorkflowHelper
{
    public const string WorkflowPermissionKeyPrefix = ElsaModulePermissions.Workflow.Default;

    public static string GenerateWorkflowPermissionKey(Guid guid)
    {
        return $"{WorkflowPermissionKeyPrefix}.{guid.ToString("d")}";
    }

    public static string GenerateWorkflowPermissionKey(WorkflowDefinition workflow)
    {
        return $"{WorkflowPermissionKeyPrefix}.{workflow.Id.ToString("d")}";
    }

    public static Guid ResolveWorkflowIdFromPermissionKey(string key)
    {
        return Guid.Parse(key.Substring(WorkflowPermissionKeyPrefix.Length + 1));
    }
}
