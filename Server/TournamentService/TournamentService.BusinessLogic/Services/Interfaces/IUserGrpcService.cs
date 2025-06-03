using System;

namespace TournamentService.BusinessLogic.Services.Interfaces;

public interface IUserGrpcService
{
    public Task AddToUser(string userId, string tournamentId);
    public Task RemoveFromUser(string userId, string tournamentId);
}
