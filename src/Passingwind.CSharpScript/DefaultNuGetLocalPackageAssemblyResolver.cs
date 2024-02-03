using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Repositories;

namespace Passingwind.CSharpScriptEngine;

public class DefaultNuGetLocalPackageAssemblyResolver : INuGetLocalPackageAssemblyResolver
{
    private readonly NuGetv3LocalRepository _localRepository;

    private readonly CSharpScriptEngineOptions _engineOptions;

    public DefaultNuGetLocalPackageAssemblyResolver(IOptions<CSharpScriptEngineOptions> engineOptions)
    {
        _engineOptions = engineOptions.Value;

        _localRepository = new NuGetv3LocalRepository(_engineOptions.NuGetCachePath);
    }

    public async Task<IEnumerable<string>> GetReferencesAsync(PackageIdentity package, NuGetFramework framework, CancellationToken cancellationToken = default)
    {
        if (!_localRepository.Exists(package.Id, package.Version))
        {
            throw new InvalidOperationException($"The package '{package}' not found.");
        }

        var packageInfo = _localRepository.FindPackage(package.Id, package.Version);

        var file = packageInfo.ZipPath!;

        var root = Path.GetDirectoryName(file);

        using var archiveReader = new PackageArchiveReader(file);

        var supportedFrameworks = await archiveReader.GetSupportedFrameworksAsync(cancellationToken);
        var referenceItemGroups = await archiveReader.GetReferenceItemsAsync(cancellationToken);
        // var dependencyGroups = await archiveReader.GetPackageDependenciesAsync(cancellationToken);

        var packageTargetFramework = NuGetFrameworkUtility.GetNearest(supportedFrameworks, framework, x => x);

        var targetReferenceItemGroup = referenceItemGroups.FirstOrDefault(x => x.TargetFramework == packageTargetFramework);

        if (targetReferenceItemGroup == null)
        {
            return new List<string>();
        }

        var referenceFiles = targetReferenceItemGroup.Items?.Select(x => Path.Combine(root!, x)).ToList();

        return referenceFiles ?? new List<string>();
    }
}
