using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.Design;

public class RuntimePropertyVisibleDependsOnSettings
{
    public RuntimePropertyVisibleDependsOnSettings(string[] dependsOn, Dictionary<string, object> when)
    {
        DependsOn = dependsOn;
        When = when;
    }

    public string[] DependsOn { get; }
    public Dictionary<string, object> When { get; }
}
