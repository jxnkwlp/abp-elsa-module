using System;
using System.Threading;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;

namespace Passingwind.Abp.ElsaModule.Common;

public class GlobalVariableDomainService : DomainService
{
    private readonly IGlobalVariableRepository _globalVariableRepository;
    private readonly IDistributedCache<GlobalVariableItem> _cache;

    public GlobalVariableDomainService(IGlobalVariableRepository globalVariableRepository, IDistributedCache<GlobalVariableItem> cache)
    {
        _globalVariableRepository = globalVariableRepository;
        _cache = cache;
    }

    public virtual async System.Threading.Tasks.Task<string> GetValueFromCacheAsync(string key, CancellationToken cancellationToken = default)
    {
        // always set on cache 
        var cacheItem = await _cache.GetOrAddAsync(key, async () =>
        {
            var entity = await _globalVariableRepository.FindAsync(key, cancellationToken);

            return new GlobalVariableItem()
            {
                Value = entity?.Value,
            };
        },
        () => new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)
        });

        return cacheItem?.Value;
    }
}

public class GlobalVariableItem
{
    public string Value { get; set; }
}
