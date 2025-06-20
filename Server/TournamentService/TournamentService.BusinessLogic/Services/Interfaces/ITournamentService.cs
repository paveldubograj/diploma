using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using TournamentService.BusinessLogic.Models.Filters;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Models.Tournament;
using TournamentService.Shared.Enums;

namespace TournamentService.BusinessLogic.Services.Interfaces;

public interface ITournamentService
{
    public Task<List<TournamentCleanDto>> GetAllByPageAsync(int page, int pageSize);
    public Task<TournamentPagedResponse> GetByFilterAsync(TournamentFilter filter);
    public Task<TournamentDto> GetByIdAsync(string id);
    public Task<TournamentDto> DeleteAsync(string id, string userId, bool isAdmin = false);
    public Task<TournamentDto> UpdateAsync(string id, TournamentDto newsDto, string userId, bool isAdmin = false);
    public Task<TournamentDto> AddAsync(TournamentCreateDto newsDto, string id);
    public Task SetWinnerForMatchAsync(string tournamentId, string matchId, string winnerId, string looserId, int winPoints, int loosePoints, string userId);
    public Task SetNextRound(string tournamentId, string userId);
    public Task<TournamentCleanDto> EndTournamentAsync(string tournamentId, string userId);
    public Task GenerateBracketAsync(string tournamentId, string userId);
    public Task<TournamentCleanDto> StartTournamentAsync(string tournamentId, string userId);
    public Task<int> GetTotalAsync();
    public Task<TournamentDto> AddImageAsync(string id, IFormFile file, string userId);
    public Task<TournamentDto> RemoveImageAsync(string id, string userId);
    public Task<TournamentPagedResponse> GetByOwnerAsync(string ownerId, int page, int pageSize);
    public Task<TournamentPagedResponse> GetByIdsAsync(List<string> ids);
}
