using System;

namespace TournamentService.BusinessLogic.Services.Interfaces;

public interface ICacheService
{
    public Task<T?> GetAsync<T>(string key);
    public Task SetAsync<T>(string key, T value);
}
