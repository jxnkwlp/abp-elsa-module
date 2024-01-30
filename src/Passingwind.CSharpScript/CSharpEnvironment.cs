using System;
using System.Collections.Generic;

namespace Passingwind.CSharpScriptEngine;

public static class CSharpEnvironment
{
    public static Dictionary<string, string> TargetFrameworkMaps { get; } = new Dictionary<string, string>
    {
        { ".NETCoreApp,Version=v6.0", "net6.0" },
        { ".NETCoreApp,Version=v7.0", "net7.0" },
        { ".NETCoreApp,Version=v8.0", "net8.0" },
    };

    public static string GetCurrentShortTargetFramework()
    {
        // TODO
        // use NuGetFramework?

        if (TargetFrameworkMaps.TryGetValue(AppContext.TargetFrameworkName!, out var name))
        {
            return name;
        }

        throw new InvalidOperationException("Unknow runtime framework");
    }
}
