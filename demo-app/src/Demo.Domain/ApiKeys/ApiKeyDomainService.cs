using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Demo.ApiKeys;

public class ApiKeyDomainService : DomainService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IApiKeyRepository _apiRepository;

    public ApiKeyDomainService(IDistributedCache distributedCache, IApiKeyRepository apiRepository)
    {
        _distributedCache = distributedCache;
        _apiRepository = apiRepository;
    }

    public Task<string> GenerateSecretAsync(ApiKey key)
    {
        return Task.FromResult($"AK-{Guid.NewGuid()}");
    }

    public virtual async Task<Guid?> ValidateAsync(string value)
    {
        string cacheKey = $"apikey:cache:{value}:userid";
        var userIdString = await _distributedCache.GetStringAsync(cacheKey);

        if (string.IsNullOrWhiteSpace(userIdString))
        {
            var apiKey = await _apiRepository.FirstOrDefaultAsync(X => X.Secret == value);
            if (apiKey != null)
            {
                if (apiKey.ExpirationTime.HasValue && apiKey.ExpirationTime < Clock.Now)
                    return null;

                await _distributedCache.SetStringAsync(cacheKey, apiKey.UserId.ToString(), new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = apiKey.ExpirationTime.HasValue ? new DateTimeOffset(apiKey.ExpirationTime.Value.ToUniversalTime()) : DateTimeOffset.UtcNow.AddHours(6),
                });

                return apiKey.UserId;
            }
        }
        else
        {
            if (Guid.TryParse(userIdString, out Guid userId2))
                return userId2;
        }

        return null;
    }
}
