using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Passingwind.CSharpScriptEngine.Roslyn;

public class RoslynProject
{
    public string Name { get; }
    public ProjectId Id { get; }

    public ImmutableArray<Assembly> Assemblies { get; }
    public ImmutableArray<string> Imports { get; }

    public ImmutableArray<MetadataReference> ResolvedReferences => GetResolvedReferences();

    public ImmutableArray<MetadataReference> AllMetadataReferences => RoslynHost.DefaultReferences.Concat(Assemblies.Select(x => GetMetadataReference(x))).Concat(ResolvedReferences).ToImmutableArray();
    public ImmutableArray<string> AllImports => RoslynHost.DefaultImports.Concat(Imports).Concat(Imports);

    private readonly Dictionary<string, ImmutableArray<MetadataReference>> _documentResolvedReferences = new Dictionary<string, ImmutableArray<MetadataReference>>();

    public RoslynProject(string name, ProjectId id, ImmutableArray<Assembly> assemblies, ImmutableArray<string> imports)
    {
        Name = name;
        Id = id;
        Assemblies = assemblies;
        Imports = imports;
    }

    public override string ToString()
    {
        return Id.ToString();
    }

    public void UpdateDocumentMetadataReferences(string name, IEnumerable<MetadataReference> metadataReferences)
    {
        lock (_documentResolvedReferences)
        {
            _documentResolvedReferences[name] = metadataReferences.ToImmutableArray();
        }
    }

    protected ImmutableArray<MetadataReference> GetResolvedReferences()
    {
        return _documentResolvedReferences.Values.SelectMany(x => x).ToImmutableArray();
    }

    private static MetadataReference GetMetadataReference(Assembly assembly)
    {
        return MetadataReference.CreateFromFile(assembly.Location);
    }
}
