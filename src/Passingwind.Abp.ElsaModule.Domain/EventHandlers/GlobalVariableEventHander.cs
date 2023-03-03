using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace Passingwind.Abp.ElsaModule.EventHandlers;

public class GlobalVariableEventHander :
    ILocalEventHandler<EntityCreatedEventData<GlobalVariable>>,
    ILocalEventHandler<EntityUpdatedEventData<GlobalVariable>>,
    ILocalEventHandler<EntityDeletedEventData<GlobalVariable>>,
    ITransientDependency
{
    private readonly ILogger<GlobalVariableEventHander> _logger;
    private readonly IDistributedCache<GlobalVariableItem> _cache;

    public GlobalVariableEventHander(ILogger<GlobalVariableEventHander> logger, IDistributedCache<GlobalVariableItem> cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task HandleEventAsync(EntityDeletedEventData<GlobalVariable> eventData)
    {
        _logger.LogDebug($"Global variable cache with key '{eventData.Entity.Key}' removed.");

        await _cache.RemoveAsync(eventData.Entity.Key);
    }

    public async Task HandleEventAsync(EntityUpdatedEventData<GlobalVariable> eventData)
    {
        _logger.LogDebug($"Global variable cache with key '{eventData.Entity.Key}' removed.");

        await _cache.RemoveAsync(eventData.Entity.Key);
    }

    public async Task HandleEventAsync(EntityCreatedEventData<GlobalVariable> eventData)
    {
        _logger.LogDebug($"Global variable cache with key '{eventData.Entity.Key}' removed.");

        await _cache.RemoveAsync(eventData.Entity.Key);
    }
}
