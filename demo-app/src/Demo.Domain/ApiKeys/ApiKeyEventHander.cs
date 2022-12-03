using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus.Distributed;

namespace Demo.ApiKeys;

public class ApiKeyEventHander : IDistributedEventHandler<EntityDeletedEventData<ApiKey>>, ITransientDependency
{
    private readonly IDistributedCache _distributedCache;

    public ApiKeyEventHander(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task HandleEventAsync(EntityDeletedEventData<ApiKey> eventData)
    {
        string cacheKey = $"apikey:cache:{eventData.Entity.Secret}:userid";

        await _distributedCache.RemoveAsync(cacheKey);
    }
}
