using System;

namespace NewsService.BusinessLogic.Services.Interfaces;

public interface IDisciplineGrpcService
{
    public Task<bool> IsDisciplineExists(string id);
}
