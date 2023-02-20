using System.Collections.Generic;
using System.Reflection;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public class CSharpTypeDefinition
{
    public string Text { get; set; }
    public List<string> Imports { get; set; }
    public List<Assembly> Assemblies { get; set; }
}
