using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Passingwind.CSharpScriptEngine.References;

public class NuGetReferenceResolver : IScriptDirectiveReferenceResolver<NuGetDirectiveReference>
{
    private readonly INuGetPackageService _nuGetPackageService;

    public NuGetReferenceResolver(INuGetPackageService nuGetPackageService)
    {
        _nuGetPackageService = nuGetPackageService;
    }

    public async Task<ImmutableArray<MetadataReference>> GetReferencesAsync(NuGetDirectiveReference reference, CancellationToken cancellationToken = default)
    {
        if (reference is null)
        {
            throw new ArgumentNullException(nameof(reference));
        }

        var files = await _nuGetPackageService.GetReferencesAsync(
            packageId: reference.PackageId,
            version: reference.Version,
            targetFramework: AppContext.TargetFrameworkName!,
            resolveDependency: true,
            cancellationToken: cancellationToken);

        if (files.Any())
        {
            return files.Select(x => MetadataReference.CreateFromFile(x)).ToImmutableArray<MetadataReference>();
        }

        return ImmutableArray<MetadataReference>.Empty;
    }
}
