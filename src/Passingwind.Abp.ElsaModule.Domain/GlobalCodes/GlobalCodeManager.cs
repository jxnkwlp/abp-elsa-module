using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

public class GlobalCodeManager : DomainService
{
    private readonly IGlobalCodeRepository _globalCodeRepository;
    private readonly IGlobalCodeContentRepository _globalCodeContentRepository;
    private readonly IGlobalCodeVersionRepository _globalCodeVersionRepository;
    private readonly IDistributedCache<GlobalCodeContentCacheItem> _globalCodeContentDistributedCache;

    public GlobalCodeManager(IGlobalCodeRepository globalCodeRepository, IGlobalCodeContentRepository globalCodeContentRepository, IDistributedCache<GlobalCodeContentCacheItem> globalCodeContentDistributedCache, IGlobalCodeVersionRepository globalCodeVersionRepository)
    {
        _globalCodeRepository = globalCodeRepository;
        _globalCodeContentRepository = globalCodeContentRepository;
        _globalCodeContentDistributedCache = globalCodeContentDistributedCache;
        _globalCodeVersionRepository = globalCodeVersionRepository;
    }

    public async Task<string> GetContentByVersionAsync(string name, int version, CancellationToken cancellationToken = default)
    {
        var cacheItem = await _globalCodeContentDistributedCache.GetOrAddAsync($"{name}:{version}", async () =>
        {
            var entity = await _globalCodeRepository.GetByNameAsync(name);

            var contentEntity = await _globalCodeContentRepository.FindByVersionAsync(entity.Id, version);

            return new GlobalCodeContentCacheItem(entity.Name, entity.Language, entity.Type, contentEntity.Id, contentEntity.Version, contentEntity.Content);
        }, token: cancellationToken);

        return cacheItem.Content;
    }

    public async Task<string> GetContentAsync(string name, CancellationToken cancellationToken = default)
    {
        var cacheItem = await _globalCodeContentDistributedCache.GetOrAddAsync($"{name}:latest", async () =>
        {
            var entity = await _globalCodeRepository.GetByNameAsync(name);

            var contentEntity = await _globalCodeContentRepository.FindByVersionAsync(entity.Id, entity.LatestVersion);

            return new GlobalCodeContentCacheItem(entity.Name, entity.Language, entity.Type, contentEntity.Id, contentEntity.Version, contentEntity.Content);
        }, token: cancellationToken);

        return cacheItem.Content;
    }

    public async Task UpdateContentAsync(GlobalCode code, string content, CancellationToken cancellationToken = default)
    {
        if (await _globalCodeContentRepository.IsVersionExistsAsync(code.Id, code.LatestVersion, cancellationToken))
        {
            await _globalCodeContentRepository.UpdateContentAsync(code.Id, code.LatestVersion, content, cancellationToken);
        }
        else
        {
            await _globalCodeContentRepository.InsertAsync(new GlobalCodeContent(GuidGenerator.Create(), code.Id, code.LatestVersion, content, CurrentTenant.Id), cancellationToken: cancellationToken);
            await _globalCodeVersionRepository.InsertAsync(new GlobalCodeVersion(GuidGenerator.Create(), code.Id, code.LatestVersion), cancellationToken: cancellationToken);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        await _globalCodeVersionRepository.DeleteDirectAsync(x => x.GlobalCodeId == id);
        await _globalCodeContentRepository.DeleteDirectAsync(x => x.GlobalCodeId == id);
        await _globalCodeRepository.DeleteAsync(id);
    }
}
