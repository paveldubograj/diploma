using System;
using TournamentService.BusinessLogic.Models.Match;

namespace TournamentService.BusinessLogic.Services.Interfaces;

public interface IMatchGrpcService
{
    public Task CreateMatches(List<MatchDto> matches);
    public Task<MatchDto> GetMatchById(string matchId);
    public Task<MatchDto> GetMatchByName(string tournamentId, string name);
    public Task UpdateMatch(string matchId, MatchDto match);
}
