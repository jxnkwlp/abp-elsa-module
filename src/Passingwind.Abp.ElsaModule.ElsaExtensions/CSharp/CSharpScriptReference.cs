using System.Collections.Generic;
using System.Reflection;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class CSharpScriptReference
{
    public CSharpScriptReference()
    {
        Assemblies = new List<Assembly>();
        Imports = new List<string>();
    }

    public List<Assembly> Assemblies { get; }
    public List<string> Imports { get; }
}
