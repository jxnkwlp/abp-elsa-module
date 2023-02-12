using Volo.Abp.Settings;

namespace Passingwind.WorkflowApp.Settings;

public class WorkflowAppSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(WorkflowAppSettings.MySetting1));
    }
}
