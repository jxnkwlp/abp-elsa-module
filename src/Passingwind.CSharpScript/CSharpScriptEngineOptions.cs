using System;
using System.Collections.Generic;

namespace Passingwind.CSharpScriptEngine;

public class CSharpScriptEngineOptions
{
    public string NuGetCachePath { get; set; }
    public List<string> NuGetServer { get; }

    public CSharpScriptEngineOptions()
    {
        NuGetServer = new List<string>();
#if DEBUG
        NuGetCachePath = System.IO.Path.Combine(AppContext.BaseDirectory, ".nuget");
#else
        NuGetCachePath = NuGet.Configuration.SettingsUtility.GetGlobalPackagesFolder(NuGet.Configuration.Settings.LoadDefaultSettings(AppContext.BaseDirectory));
#endif
    }
}
