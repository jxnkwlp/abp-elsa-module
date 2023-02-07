using System.Collections.Generic;
using System.Reflection;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;
public class CSharpSourceGeneratoResult
{
    public string Code { get; set; }
    public List<Assembly> Assemblies { get; set; }
    public List<string> Imports { get; set; }
}
