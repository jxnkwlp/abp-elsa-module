using Passingwind.WorkflowApp.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.SettingManagement;

namespace Passingwind.WorkflowApp.Permissions;

public class OAuth2SettingsPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var settings = context.GetGroupOrNull(SettingManagementPermissions.GroupName);

        if (settings != null)
        {
            settings.AddPermission(OAuth2SettingsPermissions.OAuth2, L("Permission:OAuth2"));
        }
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkflowAppResource>(name);
    }
}
