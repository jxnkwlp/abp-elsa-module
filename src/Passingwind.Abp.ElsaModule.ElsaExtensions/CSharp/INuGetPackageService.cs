using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.CSharp;

public interface INuGetPackageService : ISingletonDependency
{
    string NuGetCacheFolder { get; set; }

    Task<IEnumerable<string>> SearchAsync(string name, int resultCount = 10, CancellationToken cancellationToken = default);
    Task DownloadAsync(string packageId, string version, string tagetFrameworkName, bool dependency = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetPackageVersionsAsync(string packageId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetReferencesAsync(string packageId, string version, string tagetFrameworkName, bool resolveDependency = false, bool downloadPackage = false, CancellationToken cancellationToken = default);
}