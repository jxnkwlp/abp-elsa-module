using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.CSharpScriptEngine;

public interface INuGetPackageService
{
    Task<IEnumerable<string>> SearchAsync(string name, int resultCount = 10, CancellationToken cancellationToken = default);
    Task DownloadAsync(string packageId, string version, string tagetFrameworkName, bool dependency = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetPackageVersionsAsync(string packageId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetReferencesAsync(string packageId, string version, string tagetFrameworkName, bool resolveDependency = false, bool downloadPackage = false, CancellationToken cancellationToken = default);
}
