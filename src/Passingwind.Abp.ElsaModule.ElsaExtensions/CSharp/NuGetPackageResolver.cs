using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class NuGetPackageResolver : INuGetPackageResolver
{
    private readonly ILogger<NuGetPackageResolver> _logger;
    private readonly INuGetPackageService _packageService;

    public NuGetPackageResolver(ILogger<NuGetPackageResolver> logger, INuGetPackageService packageService)
    {
        _logger = logger;
        _packageService = packageService;
    }

    public async Task<NuGetPackageReference> ResolveAsync(string text, CancellationToken cancellationToken = default)
    {
        if (NuGetPackageReference.TryParse(text, out var package))
        {
            // restore
            await _packageService.DownloadAsync(package.PackageId, package.PackageVersion, CSharpEnvironment.TargetFramework, true, cancellationToken);

            return package;
        }
        return null;
    }
}
