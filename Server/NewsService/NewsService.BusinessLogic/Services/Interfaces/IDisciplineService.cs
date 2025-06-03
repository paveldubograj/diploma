using System;

namespace NewsService.BusinessLogic.Services.Interfaces;

public interface IDisciplineService
{
    public Task<bool> IsDisciplineExists(string id);
}
