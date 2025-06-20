using System;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TournamentService.BusinessLogic.Services.Interfaces;

namespace TournamentService.BusinessLogic.Services;

public class CacheService : ICacheService
{
    public IDistributedCache _cache;
    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _cache.GetStringAsync(key);
        
        if (value is null)
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(value);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var serializedValue = JsonConvert.SerializeObject(value);
        await _cache.SetStringAsync(key, serializedValue, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                });
    }
}
