using Volo.Abp.Caching;

namespace Passingwind.Abp.ElsaModule.Activities.Caching;

[CacheName("CacheActivity")]
public class CacheActivityCacheItem
{
    public string Value { get; set; }
}
