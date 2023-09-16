using System;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class CSharpScriptMetadataResolver : MetadataReferenceResolver
{
    public static CSharpScriptMetadataResolver Instance => new CSharpScriptMetadataResolver(ScriptMetadataResolver.Default);

    private readonly ScriptMetadataResolver _resolver;

    public CSharpScriptMetadataResolver(ScriptMetadataResolver resolver)
    {
        _resolver = resolver;
    }

    public override bool Equals(object other) => _resolver.Equals(other);

    public override int GetHashCode() => _resolver.GetHashCode();

    public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string baseFilePath, MetadataReferenceProperties properties)
    {
        // #r nuget:
        if (reference.StartsWith("nuget", StringComparison.OrdinalIgnoreCase))
        {
            // HACK We need to return something here to "mark" the reference as resolved. 
            // https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/ReferenceManager/CommonReferenceManager.Resolution.cs#L838
            return ImmutableArray<PortableExecutableReference>.Empty.Add(MetadataReference.CreateFromFile(typeof(CSharpScriptMetadataResolver).GetTypeInfo().Assembly.Location));
        }

        return _resolver.ResolveReference(reference, baseFilePath, properties);
    }
}
