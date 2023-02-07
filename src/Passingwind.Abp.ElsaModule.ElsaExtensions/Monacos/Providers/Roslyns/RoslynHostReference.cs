using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Roslyns;

public class RoslynHostReference
{
    private readonly IEnumerable<string> _imports;

    private static ImmutableArray<string> DefaultImports => ImmutableArray.Create(new[] {
        "System",
        "System.Console",
        "System.Collections",
        "System.Collections.Generic",
        "System.Collections.Concurrent",
        "System.Dynamic",
        "System.Linq",
        "System.Globalization",
        "System.IO",
        "System.Text",
        "System.Text.Encoding",
        "System.Text.RegularExpressions",
        "System.Threading",
        "System.Threading.Tasks",
        "System.Threading.Tasks.Parallel",
        "System.Threading.Thread",
        "System.ValueTuple",
        "System.Reflection",
    });

    private static ImmutableArray<MetadataReference> CreateDefaultReferences()
    {
        var references = new List<MetadataReference>()
        {
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
            MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Collections.Hashtable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.ComponentModel.DescriptionAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Data.DataSet).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Xml.XmlDocument).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.ComponentModel.INotifyPropertyChanged).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Linq.Expressions.Expression).Assembly.Location)
        };

        return references.ToImmutableArray();
    }

    public IEnumerable<Assembly> Assemblies { get; }

    public RoslynHostReference(IEnumerable<Assembly> assemblies, IEnumerable<string> imports)
    {
        _imports = imports;
        Assemblies = assemblies;
    }

    public RoslynHostReference()
    {
        _imports = new List<string>();
        Assemblies = new List<Assembly>();
    }

    public ImmutableArray<MetadataReference> GetReferences()
    {
        var refs = Assemblies.Select(x => MetadataReference.CreateFromFile(x.Location));
        return CreateDefaultReferences().AddRange(refs);
    }

    public ImmutableArray<string> GetImports() => DefaultImports.AddRange(_imports.ToImmutableArray());

}
