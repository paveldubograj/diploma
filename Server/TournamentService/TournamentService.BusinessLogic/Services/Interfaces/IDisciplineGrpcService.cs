using System;

namespace TournamentService.BusinessLogic.Services.Interfaces;

public interface IDisciplineGrpcService
{
    public Task<bool> IsDisciplineExists(string id);
}
