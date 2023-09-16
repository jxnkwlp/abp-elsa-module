using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Repositories;
using NuGet.Versioning;
using Volo.Abp;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class NuGetPackageService : INuGetPackageService
{
    private string _nuGetCacheFolder;

    private readonly NuGet.Common.ILogger _nuGetLogger = NuGet.Common.NullLogger.Instance;
    private readonly SourceCacheContext _sourceCacheContext;
    private readonly ILogger<NuGetPackageService> _logger;
    private NuGetv3LocalRepository _localRepository;

    public string NuGetCacheFolder
    {
        get
        {
#if DEBUG
            return Path.Combine(Directory.GetCurrentDirectory(), ".nuget");
#else
            if (string.IsNullOrEmpty(_nuGetCacheFolder))
                return SettingsUtility.GetGlobalPackagesFolder(NuGet.Configuration.Settings.LoadDefaultSettings(Directory.GetCurrentDirectory()));
            else
                return _nuGetCacheFolder;
#endif
        }
        set
        {
            _localRepository = new NuGetv3LocalRepository(value);
            _nuGetCacheFolder = value;
        }
    }

    protected IEnumerable<SourceRepository> Repositories { get; }

    public NuGetPackageService(NuGetLogger nuGetPackageLogger, ILogger<NuGetPackageService> logger)
    {
        _sourceCacheContext = new SourceCacheContext() { IgnoreFailedSources = true, MaxAge = System.DateTimeOffset.Now.AddHours(1), };
        // initial
        Repositories = GetSourceRepositories();

        _nuGetLogger = nuGetPackageLogger;
        _localRepository = new NuGetv3LocalRepository(NuGetCacheFolder);
        _logger = logger;
    }

    public async Task<IEnumerable<string>> GetReferencesAsync(string packageId, string version, string tagetFrameworkName, bool resolveDependency = false, bool downloadPackage = false, CancellationToken cancellationToken = default)
    {
        var packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(version));
        // project target framework
        var targetFramework = NuGetFramework.Parse(tagetFrameworkName);

        _logger.LogDebug("Starting resolve package '{packageId}' references ", packageIdentity);

        var reposities = Repositories;

        using MemoryStream nupkgStream = await GetNupkgStreamAsync(reposities, packageIdentity, cancellationToken);

        if (nupkgStream.Length == 0)
        {
            throw new UserFriendlyException($"The nuget package {packageIdentity} not found.");
        }

        var nuGetAllReferences = new List<NuGetReference>();

        await GetReferencesAsync(nuGetAllReferences, reposities, nupkgStream, packageIdentity, targetFramework, resolveDependency, downloadPackage, cancellationToken);

        // TODO 
        var result = new List<NuGetReference>();
        foreach (var item in nuGetAllReferences)
        {
            if (!result.Any(x => x.Identity.Id == item.Identity.Id && x.Identity.Version >= item.Identity.Version))
            {
                result.Add(item);
            }
        }

        _logger.LogDebug("Resolved package '{packageId}' references, count: {count} ", packageIdentity, result.Count);

        return result.SelectMany(x => x.References);
    }

    public async Task<IEnumerable<string>> SearchAsync(string name, int resultCount = 10, CancellationToken cancellationToken = default)
    {
        var repository = GetSourceRepository();

        var resource = await repository.GetResourceAsync<PackageSearchResource>();

        var result = await resource.SearchAsync(name, new SearchFilter(true, null), 0, resultCount, _nuGetLogger, cancellationToken);

        return result.Select(x => x.Identity.Id).ToList();
    }

    public async Task<IEnumerable<string>> GetPackageVersionsAsync(string packageId, CancellationToken cancellationToken = default)
    {
        var repository = GetSourceRepository();

        var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

        var allVersion = await resource.GetAllVersionsAsync(packageId, _sourceCacheContext, _nuGetLogger, cancellationToken);

        return allVersion.Select(x => x.Version.ToString());
    }

    public async Task DownloadAsync(string packageId, string version, string tagetFrameworkName, bool dependency = false, CancellationToken cancellationToken = default)
    {
        var packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(version));
        var targetFramework = NuGetFramework.Parse(tagetFrameworkName);

        _logger.LogDebug("Starting download and extracte package '{packageId}'", packageIdentity);

        var reposities = Repositories;

        // local cache 
        var nupkgFile = GetNupkgFileFromCache(packageIdentity);

        if (!string.IsNullOrWhiteSpace(nupkgFile))
        {
            _logger.LogDebug("Find package '{packageId}' nupkg from file: {nupkgFile} ", packageIdentity, nupkgFile);
        }
        // not found.
        else
        {
            _logger.LogDebug("Package '{packageId}' in local cache folder not found, try find from repositories.", packageIdentity);

            var packageExtractionContext = new PackageExtractionContext(PackageSaveMode.Defaultv3, XmlDocFileSaveMode.None, null, _nuGetLogger);

            var tasks = new List<Task>();

            // download from repositiy
            foreach (var repository in reposities)
            {
                var resource = await repository.GetResourceAsync<FindPackageByIdResource>();
                if (resource == null)
                    continue;

                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    var _packageId = packageIdentity;
                    var _resource = resource;

                    // download & extract
                    await DownloadAndExtracteAsync(_resource, _packageId, packageExtractionContext, cancellationToken);
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        // dependency
        if (dependency)
        {
            var dependencyPackages = new List<PackageIdentity>();

            // resolve 
            await ResoveDependencyPackagesAsync(dependencyPackages, reposities, packageIdentity, targetFramework, cancellationToken);

            var tasks = new List<Task>();
            foreach (var item in dependencyPackages.Distinct())
            {
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    var id = item.Id;
                    var version = item.Version;
                    var package2 = new PackageIdentity(id, version);

                    // do not resolve dependencies
                    await DownloadAsync(id, version.ToNormalizedString(), tagetFrameworkName, false, cancellationToken);
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        _logger.LogDebug("Download and extracte package '{packageId}' done", packageIdentity);
    }

    private async Task DownloadAndExtracteAsync(FindPackageByIdResource resource, PackageIdentity packageId, PackageExtractionContext packageExtractionContext, CancellationToken cancellationToken = default)
    {
        if (await resource.DoesPackageExistAsync(packageId.Id, packageId.Version, _sourceCacheContext, _nuGetLogger, cancellationToken))
        {
            var downloader = await resource.GetPackageDownloaderAsync(packageId, _sourceCacheContext, _nuGetLogger, cancellationToken);

            await PackageExtractor.InstallFromSourceAsync(packageId, downloader, new VersionFolderPathResolver(NuGetCacheFolder), packageExtractionContext, cancellationToken);

            _logger.LogDebug("Package '{packageId}' installed to '{NuGetCacheFolder}' ", packageId, NuGetCacheFolder);
        }
        else
        {
            _logger.LogError("Package '{packageId}' not found", packageId);
        }
    }

    private async Task ResoveDependencyPackagesAsync(List<PackageIdentity> packages, IEnumerable<SourceRepository> repositories, PackageIdentity packageId, NuGetFramework targetFramework, CancellationToken cancellationToken = default)
    {
        foreach (var repository in repositories)
        {
            var resource = repository.GetResource<DependencyInfoResource>();

            var dependencies = await ResoveDependenciesAsync(packages, packageId, targetFramework, resource, cancellationToken);

            if (dependencies?.Any() == true)
            {
                foreach (var item in dependencies)
                {
                    if (packages.Any(x => x.Id == item.Id && x.Version == item.VersionRange.MinVersion))
                        continue;

                    var dp = new PackageIdentity(item.Id, item.VersionRange.MinVersion);
                    packages.Add(dp);

                    await ResoveDependencyPackagesAsync(packages, repositories, dp, targetFramework, cancellationToken);
                }
            }
        }
    }

    private async Task<IEnumerable<PackageDependency>> ResoveDependenciesAsync(List<PackageIdentity> packages, PackageIdentity packageId, NuGetFramework targetFramework, DependencyInfoResource resource, CancellationToken cancellationToken = default)
    {
        var packageDependencyInfo = await resource.ResolvePackage(packageId, targetFramework, _sourceCacheContext, _nuGetLogger, cancellationToken);

        if (packageDependencyInfo != null)
        {
            packages.Add(new PackageIdentity(packageDependencyInfo.Id, packageDependencyInfo.Version));

            return packageDependencyInfo.Dependencies;
        }

        return null;
    }

    private async Task GetReferencesAsync(List<NuGetReference> result, IEnumerable<SourceRepository> reposities, Stream nupkgStream, PackageIdentity packageId, NuGetFramework targetFramework, bool resolveDependency = false, bool downloadPackage = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting resolve package '{packageId}' references from nupkg ", packageId);

        using PackageArchiveReader archiveReader = new PackageArchiveReader(nupkgStream);

        var referenceItemGroups = await archiveReader.GetReferenceItemsAsync(cancellationToken);
        var supportedFrameworks = await archiveReader.GetSupportedFrameworksAsync(cancellationToken);
        var dependencyGroups = await archiveReader.GetPackageDependenciesAsync(cancellationToken);

        // find package target framework 
        var packageTargetFramework = NuGetFrameworkUtility.GetNearest(supportedFrameworks, targetFramework, x => x);

        var targetReferenceItemGroup = referenceItemGroups.FirstOrDefault(x => x.TargetFramework == packageTargetFramework);

        if (targetReferenceItemGroup == null)
            return;

        var referenceFiles = targetReferenceItemGroup.Items?.Select(x => Path.Combine(NuGetCacheFolder, packageId.Id.ToLowerInvariant(), packageId.Version.ToNormalizedString(), x)).ToList();

        if (referenceFiles != null)
        {
            result.Add(new NuGetReference(packageId, referenceFiles.ToImmutableArray()));

            if (referenceFiles.Any(x => !File.Exists(x)) && downloadPackage)
            {
                await DownloadAsync(packageId.Id, packageId.Version.ToNormalizedString(), targetFramework.GetShortFolderName(), false, cancellationToken);
            }
        }

        // var packagePathResolver = new PackagePathResolver(Path.GetFullPath("packages"));

        if (resolveDependency && dependencyGroups.Any())
        {
            await ResolveDependencyReferenceAsync(result, reposities, dependencyGroups, targetFramework, downloadPackage, cancellationToken);
        }
    }

    private async Task ResolveDependencyReferenceAsync(List<NuGetReference> result, IEnumerable<SourceRepository> reposities, IEnumerable<PackageDependencyGroup> dependencyGroups, NuGetFramework targetFramework, bool downloadPackage = false, CancellationToken cancellationToken = default)
    {
        var packageDependencyGroup = NuGetFrameworkUtility.GetNearest(dependencyGroups, targetFramework);

        if (packageDependencyGroup?.Packages?.Any() == true)
        {
            foreach (var packageDependency in packageDependencyGroup.Packages)
            {
                // TODO: VersionRange
                var packageId = new PackageIdentity(packageDependency.Id, packageDependency.VersionRange.MinVersion);

                var packageStream = await GetNupkgStreamAsync(reposities, packageId, cancellationToken);

                if (packageStream.Length == 0)
                {
                    throw new UserFriendlyException($"The nuget package {packageId} not found.");
                }

                await GetReferencesAsync(result, reposities, packageStream, packageId, targetFramework, true, downloadPackage, cancellationToken);
            }
        }
    }

    private async Task<MemoryStream> GetNupkgStreamAsync(IEnumerable<SourceRepository> reposities, PackageIdentity packageId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting download package '{packageId}' nupkg ", packageId);

        MemoryStream nupkgStream = new MemoryStream();

        //  local cache
        var nupkgFile = GetNupkgFileFromCache(packageId);
        if (!string.IsNullOrWhiteSpace(nupkgFile))
        {
            var fileBytes = File.ReadAllBytes(nupkgFile);
            nupkgStream = new MemoryStream(fileBytes);

            _logger.LogDebug("Find package '{packageId}' nupkg from file: {nupkgFile} ", packageId, nupkgFile);
        }
        // from reposity
        else
        {
            foreach (var repository in reposities)
            {
                var resource = await repository.GetResourceAsync<FindPackageByIdResource>();
                if (resource == null)
                    continue;

                if (await resource.CopyNupkgToStreamAsync(packageId.Id, packageId.Version, nupkgStream, _sourceCacheContext, _nuGetLogger, cancellationToken))
                {
                    _logger.LogDebug("Download package '{packageId}' nupkg from '{repository}' ", packageId, repository);

                    break;
                }
            }
        }

        return nupkgStream;
    }

    private string GetNupkgFileFromCache(PackageIdentity packageId)
    {
        var package = _localRepository.FindPackage(packageId.Id, packageId.Version);

        return package?.ZipPath;
    }

    protected SourceRepository GetSourceRepository()
    {
        var settings = NuGet.Configuration.Settings.LoadDefaultSettings(Directory.GetCurrentDirectory());

        var packageSourceProvider = new PackageSourceProvider(settings);

        var packageSource = packageSourceProvider.LoadPackageSources().First();

        return Repository.Factory.GetCoreV3(packageSource);
    }

    protected IEnumerable<SourceRepository> GetSourceRepositories()
    {
        var settings = NuGet.Configuration.Settings.LoadDefaultSettings(Directory.GetCurrentDirectory());

        var packageSourceProvider = new PackageSourceProvider(settings);

        var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, Repository.Provider.GetCoreV3());

        return sourceRepositoryProvider.GetRepositories();
    }
}
