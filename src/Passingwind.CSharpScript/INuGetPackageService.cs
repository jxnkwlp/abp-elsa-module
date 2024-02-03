using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.Core;

namespace Passingwind.CSharpScriptEngine;

public interface INuGetPackageService
{
    /// <summary>
    ///  Search packages from repositores
    /// </summary>
    Task<IReadOnlyList<string>> SearchAsync(string filter, int resultCount = 10, CancellationToken cancellationToken = default);

    /// <summary>
    ///  Get package all version from repositores
    /// </summary>
    Task<IReadOnlyList<string>> GetVersionsAsync(string packageId, CancellationToken cancellationToken = default);

    /// <summary>
    ///  Download package into local from repositores
    /// </summary>
    Task DownloadAsync(string packageId, string version, CancellationToken cancellationToken = default);

    /// <summary>
    ///  Get package dependencies to the specified framework
    /// </summary>
    Task<IReadOnlyList<PackageIdentity>> GetDependenciesAsync(string packageId, string version, string targetFramework, CancellationToken cancellationToken = default);

    /// <summary>
    ///  Get package references to the specified framework
    /// </summary>
    Task<IReadOnlyList<string>> GetReferencesAsync(string packageId, string version, string targetFramework, bool resolveDependency = true, CancellationToken cancellationToken = default);
}
