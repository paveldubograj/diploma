using System;

namespace TournamentService.BusinessLogic.Services.Tournaments.Interfaces;

public interface IDoubleEliminationBracket
{
    public Task GenerateBracket(string tournamentId);
    public Task GenerateLowerBracket(string tournamentId);
    public Task HandleMatchResult(string matchId, string winnerId, string looserId, int winPoints, int loosePoints);
}
