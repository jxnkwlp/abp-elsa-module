using System.Collections.Generic;
using Passingwind.Abp.ElsaModule.CSharp;

namespace Passingwind.Abp.ElsaModule.Roslyn;

public class RoslynDocumentReference
{
    public RoslynDocumentReference(string name, List<CSharpPackageReference> references)
    {
        Name = name;
        References = references;
    }

    public string Name { get; }
    public List<CSharpPackageReference> References { get; }
}
