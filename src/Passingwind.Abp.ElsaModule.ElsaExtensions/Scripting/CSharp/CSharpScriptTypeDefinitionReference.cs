using System.Collections.Generic;
using System.Reflection;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public class CSharpScriptTypeDefinitionReference
{
    public CSharpScriptTypeDefinitionReference()
    {
        Assemblies = new List<Assembly>();
        Imports = new List<string>();
    }

    public List<Assembly> Assemblies { get; }
    public List<string> Imports { get; }
}
