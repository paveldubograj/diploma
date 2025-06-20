using System;

namespace TournamentService.BusinessLogic.Services.Tournaments.Interfaces;

public interface ISwissBracket
{
    public Task GenerateSwissMatches(string tournamentId);
    public Task HandleMatchResult(string matchId, string winnerId, string loserId, int winScore, int looseScore);
}
