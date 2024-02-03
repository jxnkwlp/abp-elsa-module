using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Repositories;
using NuGet.Versioning;

namespace Passingwind.CSharpScriptEngine;

public class DefaultNuGetPackageService : INuGetPackageService
{
    protected IReadOnlyList<SourceRepository> Repositories { get; }

    private readonly NuGetv3LocalRepository _localRepository;
    private readonly SourceCacheContext _sourceCacheContext;
    private readonly string _nuGetCacheFolder;

    private readonly CSharpScriptEngineOptions _engineOptions;
    private readonly ILogger<DefaultNuGetPackageService> _logger;
    private readonly NuGet.Common.ILogger _nuGetLogger = NuGet.Common.NullLogger.Instance;
    private readonly INuGetLocalPackageAssemblyResolver _nuGetPackageAssemblyResolver;

    public DefaultNuGetPackageService(IOptions<CSharpScriptEngineOptions> engineOptions, ILogger<DefaultNuGetPackageService> logger, NuGet.Common.ILogger nuGetLogger, INuGetLocalPackageAssemblyResolver nuGetPackageAssemblyResolver)
    {
        _engineOptions = engineOptions.Value;
        _logger = logger;
        _nuGetLogger = nuGetLogger;
        _nuGetPackageAssemblyResolver = nuGetPackageAssemblyResolver;

        Repositories = GetSourceRepositories();
        _sourceCacheContext = new SourceCacheContext() { IgnoreFailedSources = true, MaxAge = System.DateTimeOffset.Now.AddHours(1), };
        _localRepository = new NuGetv3LocalRepository(_engineOptions.NuGetCachePath);
        _nuGetCacheFolder = _engineOptions.NuGetCachePath;
    }

    public async Task<IReadOnlyList<string>> GetReferencesAsync(string packageId, string version, string targetFramework, bool resolveDependency = true, CancellationToken cancellationToken = default)
    {
        var package = new PackageIdentity(packageId, NuGetVersion.Parse(version));

        var framework = NuGetFramework.Parse(targetFramework);

        return await GetReferencesAsync(package, framework, resolveDependency, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetReferencesAsync(PackageIdentity package, NuGetFramework targetFramework, bool resolveDependency = true, CancellationToken cancellationToken = default)
    {
        await DownloadAsync(package, cancellationToken);

        var references = new List<string>();
        references.AddRange(await _nuGetPackageAssemblyResolver.GetReferencesAsync(package, targetFramework, cancellationToken));

        // dependency
        if (resolveDependency)
        {
            var dependencies = await GetDependenciesAsync(package, targetFramework, cancellationToken);

            foreach (var item in dependencies)
            {
                references.AddRange(await GetReferencesAsync(item, targetFramework, resolveDependency, cancellationToken));
            }
        }

        return references;
    }

    public async Task<IReadOnlyList<PackageIdentity>> GetDependenciesAsync(string packageId, string version, string targetFramework, CancellationToken cancellationToken = default)
    {
        var package = new PackageIdentity(packageId, NuGetVersion.Parse(version));

        return await GetDependenciesAsync(package, NuGetFramework.Parse(targetFramework), cancellationToken);
    }

    protected async Task<IReadOnlyList<PackageIdentity>> GetDependenciesAsync(PackageIdentity package, NuGetFramework targetFramework, CancellationToken cancellationToken = default)
    {
        await DownloadAsync(package, cancellationToken);

        var file = GetPackageFile(package);

        if (!File.Exists(file))
        {
            throw new FileNotFoundException(file);
        }

        var reader = new PackageArchiveReader(file);

        return await GetDependenciesAsync(reader, targetFramework, cancellationToken);
    }

    protected async Task<List<PackageIdentity>> GetDependenciesAsync(PackageArchiveReader archiveReader, NuGetFramework targetFramework, CancellationToken cancellationToken = default)
    {
        var dependencyGroups = await archiveReader.GetPackageDependenciesAsync(cancellationToken);

        var packageDependencyGroup = NuGetFrameworkUtility.GetNearest(dependencyGroups, targetFramework);

        var result = new List<PackageIdentity>();

        if (packageDependencyGroup?.Packages?.Any() == true)
        {
            foreach (var packageDependency in packageDependencyGroup.Packages)
            {
                // TODO
                var package = new PackageIdentity(packageDependency.Id, packageDependency.VersionRange.MinVersion);

                result.Add(package);
            }
        }

        return result;
    }

    public async Task DownloadAsync(string packageId, string version, CancellationToken cancellationToken = default)
    {
        var package = new PackageIdentity(packageId, NuGetVersion.Parse(version));

        await DownloadAsync(package, cancellationToken);
    }

    public async Task DownloadAsync(PackageIdentity package, CancellationToken cancellationToken = default)
    {
        if (IsPackageSaved(package))
        {
            // _logger.LogDebug("Package '{Package}' found in local ", package);
            return;
        }

        await DownloadAndSaveAsync(package, cancellationToken);
    }

    protected async Task DownloadAndSaveAsync(PackageIdentity package, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Downloading package '{Package}' ", package);

        var packageExtractionContext = new PackageExtractionContext(PackageSaveMode.Defaultv3, XmlDocFileSaveMode.Compress, null, _nuGetLogger);

        foreach (var repository in Repositories)
        {
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();
            if (resource == null)
            {
                continue;
            }

            var _packageId = package;
            var _resource = resource;

            await SavePackageAsync(_resource, _packageId, packageExtractionContext, cancellationToken);
        }
    }

    protected async Task SavePackageAsync(FindPackageByIdResource resource, PackageIdentity package, PackageExtractionContext packageExtractionContext, CancellationToken cancellationToken = default)
    {
        if (IsPackageSaved(package))
        {
            return;
        }

        if (await resource.DoesPackageExistAsync(package.Id, package.Version, _sourceCacheContext, _nuGetLogger, cancellationToken))
        {
            _logger.LogDebug("Extracting package '{Package}' ", package);

            var downloader = await resource.GetPackageDownloaderAsync(package, _sourceCacheContext, _nuGetLogger, cancellationToken);

            await PackageExtractor.InstallFromSourceAsync(package, downloader, new VersionFolderPathResolver(_nuGetCacheFolder), packageExtractionContext, cancellationToken);

            _logger.LogDebug("Package '{Package}' installed to '{NuGetCacheFolder}' ", package, _nuGetCacheFolder);
        }
        else
        {
            _logger.LogError("Package '{Package}' not found int repository", package);
        }
    }

    public async Task<IReadOnlyList<string>> GetVersionsAsync(string packageId, CancellationToken cancellationToken = default)
    {
        var tasks = Repositories.Select(x => GetPackageVersionsAsync(x, packageId, cancellationToken));

        var result = await Task.WhenAll(tasks);

        return (IReadOnlyList<string>)result.SelectMany(x => x).Distinct().OrderByDescending(x => x);
    }

    private async Task<IReadOnlyList<string>> GetPackageVersionsAsync(SourceRepository repository, string packageId, CancellationToken cancellationToken = default)
    {
        _logger.LogError("Fech package '{PackageId}' versions in repository '{repository}'", packageId, repository);

        var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

        var allVersion = await resource.GetAllVersionsAsync(packageId, _sourceCacheContext, _nuGetLogger, cancellationToken);

        return (IReadOnlyList<string>)allVersion.Select(x => x.Version.ToString());
    }

    public async Task<IReadOnlyList<string>> SearchAsync(string filter, int resultCount = 10, CancellationToken cancellationToken = default)
    {
        var tasks = Repositories.Select(x => SearchAsync(x, filter, resultCount, cancellationToken));

        var result = await Task.WhenAll(tasks);

        return (IReadOnlyList<string>)result.SelectMany(x => x).Distinct().OrderBy(x => x);
    }

    private async Task<IReadOnlyList<string>> SearchAsync(SourceRepository repository, string filter, int resultCount = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogError("Search '{Filter}' in repository '{repository}'", filter, repository);

        var resource = await repository.GetResourceAsync<PackageSearchResource>();

        var result = await resource.SearchAsync(filter, new SearchFilter(true, null), 0, resultCount, _nuGetLogger, cancellationToken);

        return result.Select(x => x.Identity.Id).ToList();
    }

    protected bool IsPackageSaved(PackageIdentity package)
    {
        return _localRepository.Exists(package.Id, package.Version);
    }

    protected string? GetPackageFile(PackageIdentity packageId)
    {
        var package = _localRepository.FindPackage(packageId.Id, packageId.Version);

        return package?.ZipPath;
    }

    protected IReadOnlyList<SourceRepository> GetSourceRepositories()
    {
        var settings = Settings.LoadDefaultSettings(Directory.GetCurrentDirectory());

        var packageSourceProvider = new PackageSourceProvider(settings);

        var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, Repository.Provider.GetCoreV3());

        var originSources = sourceRepositoryProvider.GetRepositories();

        var result = new List<SourceRepository>();
        result.AddRange(originSources);

        if (_engineOptions.NuGetServers.Any())
        {
            foreach (var item in _engineOptions.NuGetServers)
            {
                result.Add(Repository.Factory.GetCoreV3(item));
            }
        }

        return result;
    }
}
