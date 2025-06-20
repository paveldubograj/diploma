using System;
using MatchService.BusinessLogic.Models.Filter;
using MatchService.BusinessLogic.Models.Match;
using MatchService.Shared.Enums;

namespace MatchService.BusinessLogic.Services.Interfaces;

public interface IMatchService
{
    public Task<List<MatchListDto>> GetAllByPageAsync(int page, int pageSize);
    public Task<List<MatchListDto>> GetByFilterAsync(MatchFilter filter, SortOptions? options, int page, int pageSize);
    public Task<List<MatchListDto>> GetTournamentStructureAsync(string id);
    public Task<MatchDto> GetByIdAsync(string id);
    public Task<MatchDto> DeleteAsync(string matchId, string userId);
    public Task<MatchDto> UpdateAsync(string id, MatchDto newsDto, string userId);
    public Task<MatchDto> UpdateForUserAsync(string id, MatchUpdateDto newsDto, string userId);
    public Task<MatchDto> AddAsync(MatchDto newsDto);
    public Task<MatchDto> SetWinnerAsync(string matchId, string winnerId, int winScore, int looseScore, string userId);
    Task<MatchDto> GetByRoundAsync(string tournamentId, string round);
    Task<bool> AddMatchesAsync(List<MatchDto> matches);
    public Task<int> GetTotalAsync();
}
