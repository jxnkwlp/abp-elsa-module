using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Passingwind.CSharpScriptEngine.References;

namespace Passingwind.CSharpScriptEngine;

public class DefaultScriptReferenceResolver : IScriptReferenceResolver
{
    private readonly IScriptDirectiveReferenceResolverFactory _directiveReferenceResolverFactory;

    public DefaultScriptReferenceResolver(IScriptDirectiveReferenceResolverFactory directiveReferenceResolverFactory)
    {
        _directiveReferenceResolverFactory = directiveReferenceResolverFactory;
    }

    public Task<ImmutableArray<ScriptDirectiveReference>> GetDirectivesAsync(string scripts, CancellationToken cancellationToken = default)
    {
        var result = new List<ScriptDirectiveReference>();

        using StringReader sr = new((scripts ?? "").Trim());
        string? line = string.Empty;
        while (!string.IsNullOrWhiteSpace(line = sr.ReadLine()))
        {
            var tmp = line.Trim();
            if (tmp?.StartsWith("#r") == true)
            {
                var directive = tmp[2..].Trim().Trim('"');

                //"nuget: packageId, version"
                if (directive.StartsWith("nuget:"))
                {
                    var package = directive[6..].Trim();

                    result.Add(new NuGetDirectiveReference(package));
                }
            }
            else
            {
                break;
            }
        }

        return Task.FromResult(result.ToImmutableArray());
    }

    public Task<ImmutableArray<MetadataReference>> GetReferencesAsync(string reference, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ImmutableArray<MetadataReference>> GetReferencesAsync(ScriptDirectiveReference reference, CancellationToken cancellationToken = default)
    {
        if (reference is NuGetDirectiveReference nuGetDirectiveReference)
        {
            var referenceResolver = _directiveReferenceResolverFactory.CreateResolver(nuGetDirectiveReference);

            return await referenceResolver.GetReferencesAsync(nuGetDirectiveReference, cancellationToken);
        }

        throw new NotSupportedException();
    }

    public async Task<ImmutableArray<MetadataReference>> ResolveReferencesAsync(string scripts, CancellationToken cancellationToken = default)
    {
        var directiveReferences = await GetDirectivesAsync(scripts, cancellationToken);

        var result = new List<MetadataReference>();

        foreach (var item in directiveReferences)
        {
            result.AddRange(await GetReferencesAsync(item, cancellationToken));
        }

        return result.Distinct().ToImmutableArray();
    }
}
