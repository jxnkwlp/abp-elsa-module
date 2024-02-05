/*
 * https://github.com/dotnet-script/dotnet-script/blob/master/src/Dotnet.Script.DependencyModel.Nuget/NuGetMetadataReferenceResolver.cs
 */

using System;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

namespace Passingwind.CSharpScriptEngine;

/// <summary>
/// A <see cref="MetadataReferenceResolver"/> decorator that handles
/// references to NuGet packages in scripts.
/// </summary>
public class NuGetMetadataReferenceResolver : MetadataReferenceResolver
{
    public static NuGetMetadataReferenceResolver Instance { get; } = new NuGetMetadataReferenceResolver(ScriptMetadataResolver.Default);

    private readonly MetadataReferenceResolver _metadataReferenceResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetMetadataReferenceResolver"/> class.
    /// </summary>
    /// <param name="metadataReferenceResolver">The target <see cref="MetadataReferenceResolver"/>.</param>
    public NuGetMetadataReferenceResolver(MetadataReferenceResolver metadataReferenceResolver)
    {
        _metadataReferenceResolver = metadataReferenceResolver;
    }

    public override bool Equals(object? other)
    {
        return _metadataReferenceResolver.Equals(other);
    }

    public override int GetHashCode()
    {
        return _metadataReferenceResolver.GetHashCode();
    }

    public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string? baseFilePath, MetadataReferenceProperties properties)
    {
        if (reference.StartsWith("nuget", StringComparison.OrdinalIgnoreCase) || reference.StartsWith("sdk", StringComparison.OrdinalIgnoreCase))
        {
            // HACK We need to return something here to "mark" the reference as resolved. 
            // https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/ReferenceManager/CommonReferenceManager.Resolution.cs#L838
            return ImmutableArray<PortableExecutableReference>.Empty.Add(
                MetadataReference.CreateFromFile(typeof(NuGetMetadataReferenceResolver).GetTypeInfo().Assembly.Location));
        }

        return _metadataReferenceResolver.ResolveReference(reference, baseFilePath, properties);
    }
}
