using System;

namespace TournamentService.BusinessLogic.Services.Tournaments.Interfaces;

public interface IRoundRobinBracket
{
    public Task GenerateBracket(string tournamentId);
    public Task HandleMatchResult(string matchId, string winnerId, int winPoints, int loosePoints);
}
