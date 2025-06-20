using System;

namespace NewsService.BusinessLogic.Services.Interfaces;

public interface ICacheService
{
    public Task<T?> GetAsync<T>(string key);
    public Task SetAsync<T>(string key, T value);
}
