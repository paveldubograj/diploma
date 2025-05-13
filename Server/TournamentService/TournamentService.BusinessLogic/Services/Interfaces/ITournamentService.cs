using System;
using System.Runtime.CompilerServices;
using TournamentService.BusinessLogic.Models.Filters;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Models.Tournament;
using TournamentService.Shared.Enums;

namespace TournamentService.BusinessLogic.Services.Interfaces;

public interface ITournamentService
{
    public Task<List<TournamentCleanDto>> GetAllByPageAsync(int page, int pageSize);
    public Task<List<TournamentCleanDto>> GetByFilterAsync(TournamentFilter filter, TournamentSortOptions? options, int page, int pageSize);
    public Task<TournamentDto> GetByIdAsync(string id);
    public Task<TournamentDto> DeleteAsync(string id, string userId);
    public Task<TournamentDto> UpdateAsync(string id, TournamentDto newsDto, string userId);
    public Task<TournamentDto> AddAsync(TournamentCreateDto newsDto, string id); 
    public void SetWinnerForMatchAsync(string tournamentId, string matchId, string winnerId, string looserId, int winPoints, int loosePoints, string userId);
    public void SetNextRound(string tournamentId, string userId);
    public Task<TournamentCleanDto> EndTournamentAsync(string tournamentId, string userId);
    public void GenerateBracketAsync(string tournamentId, string userId);
    public Task<TournamentCleanDto> StartTournamentAsync(string tournamentId, string userId);
    public Task<int> GetTotalAsync();
}
