using Passingwind.Abp.ElsaModule.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class ElsaModulePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ElsaModulePermissions.GroupName, L("Permission:ElsaModule"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ElsaModuleResource>(name);
    }
}
