using Passingwind.WorkflowApp.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Passingwind.WorkflowApp.Settings;

public class OAuth2SettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(
                OAuth2Setting.Enabled,
                "false",
                L("DisplayName:OAuth2.Authority"),
                L("Description:OAuth2.Authority")),

            new SettingDefinition(
                OAuth2Setting.DisplayName,
                "OAuth2",
                L("DisplayName:OAuth2.DisplayName"),
                L("Description:OAuth2.DisplayName")),

            new SettingDefinition(
                OAuth2Setting.Authority,
                "127.0.0.1",
                L("DisplayName:OAuth2.Authority"),
                L("Description:OAuth2.Authority")),

            new SettingDefinition(
                OAuth2Setting.MetadataAddress,
                "",
                L("DisplayName:OAuth2.MetadataAddress"),
                L("Description:OAuth2.MetadataAddress")),

            new SettingDefinition(
                OAuth2Setting.ClientId,
                "",
                L("DisplayName:OAuth2.ClientId"),
                L("Description:OAuth2.ClientId")),

            new SettingDefinition(
                OAuth2Setting.ClientSecret,
                "",
                L("DisplayName:OAuth2.ClientSecret"),
                L("Description:OAuth2.ClientSecret")),

            new SettingDefinition(
                OAuth2Setting.Scope,
                "",
                L("DisplayName:OAuth2.Scope"),
                L("Description:OAuth2.Scope"))

            );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkflowAppResource>(name);
    }
}
