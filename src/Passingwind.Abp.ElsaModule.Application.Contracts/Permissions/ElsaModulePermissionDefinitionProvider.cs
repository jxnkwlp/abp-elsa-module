using Passingwind.Abp.ElsaModule.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class ElsaModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1201:Use method chaining.", Justification = "<Pending>")]
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ElsaModulePermissions.GroupName, L("Permission:ElsaModule"));

        var instancePermission = myGroup.AddPermission(ElsaModulePermissions.Instances.Default, L("Permission:Instance"));
        instancePermission.AddChild(ElsaModulePermissions.Instances.Action, L("Permission:Instance.Acton"));
        instancePermission.AddChild(ElsaModulePermissions.Instances.Statistic, L("Permission:Instance.Statistic"));
        instancePermission.AddChild(ElsaModulePermissions.Instances.Data, L("Permission:Instance.Data"));
        instancePermission.AddChild(ElsaModulePermissions.Instances.Delete, L("Permission:Delete"));

        var definitionPermission = myGroup.AddPermission(ElsaModulePermissions.Definitions.Default, L("Permission:Definition"));
        definitionPermission.AddChild(ElsaModulePermissions.Definitions.CreateOrUpdateOrPublish, L("Permission:Definition.Publish"));
        definitionPermission.AddChild(ElsaModulePermissions.Definitions.Dispatch, L("Permission:Definition.Dispatch"));
        definitionPermission.AddChild(ElsaModulePermissions.Definitions.Export, L("Permission:Definition.Export"));
        definitionPermission.AddChild(ElsaModulePermissions.Definitions.Import, L("Permission:Definition.Import"));
        definitionPermission.AddChild(ElsaModulePermissions.Definitions.Delete, L("Permission:Delete"));
        definitionPermission.AddChild(ElsaModulePermissions.Definitions.ManagePermissions, L("Permission:Definition.ManagePermissions"));

        var globalVariablesPermission = myGroup.AddPermission(ElsaModulePermissions.GlobalVariables.Default, L("Permission:GlobalVariables"));
        globalVariablesPermission.AddChild(ElsaModulePermissions.GlobalVariables.Create, L("Permission:Create"));
        globalVariablesPermission.AddChild(ElsaModulePermissions.GlobalVariables.Update, L("Permission:Update"));
        globalVariablesPermission.AddChild(ElsaModulePermissions.GlobalVariables.Delete, L("Permission:Delete"));

        var workflowTeamsPermission = myGroup.AddPermission(ElsaModulePermissions.WorkflowTeams.Default, L("Permission:WorkflowTeams"));
        workflowTeamsPermission.AddChild(ElsaModulePermissions.WorkflowTeams.Create, L("Permission:Create"));
        workflowTeamsPermission.AddChild(ElsaModulePermissions.WorkflowTeams.Update, L("Permission:Update"));
        workflowTeamsPermission.AddChild(ElsaModulePermissions.WorkflowTeams.Delete, L("Permission:Delete"));
        workflowTeamsPermission.AddChild(ElsaModulePermissions.WorkflowTeams.ManagePermissions, L("Permission:WorkflowTeams.ManagePermissions"));

        var workflowGroupPermission = myGroup.AddPermission(ElsaModulePermissions.WorkflowGroups.Default, L("Permission:WorkflowGroups"));
        workflowGroupPermission.AddChild(ElsaModulePermissions.WorkflowGroups.Create, L("Permission:Create"));
        workflowGroupPermission.AddChild(ElsaModulePermissions.WorkflowGroups.Update, L("Permission:Update"));
        workflowGroupPermission.AddChild(ElsaModulePermissions.WorkflowGroups.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ElsaModuleResource>(name);
    }
}
