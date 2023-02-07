using System.Collections.Generic;
using System.Reflection;

namespace Passingwind.Abp.ElsaModule;

public class CSharpScriptOptions
{
    public CSharpScriptOptions()
    {
        Assemblies = new List<Assembly>();
        Imports = new List<string>();
    }

    public List<Assembly> Assemblies { get; }
    public List<string> Imports { get; }
}
