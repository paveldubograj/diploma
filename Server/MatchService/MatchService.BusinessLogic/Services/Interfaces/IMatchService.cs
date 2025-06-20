using System;
using System.Security.Claims;
using MatchService.BusinessLogic.Models.Filter;
using MatchService.BusinessLogic.Models.Match;
using MatchService.Shared.Enums;

namespace MatchService.BusinessLogic.Services.Interfaces;

public interface IMatchService
{
    public Task<List<MatchListDto>> GetAllByPageAsync(int page, int pageSize);
    public Task<MatchPagedResponse> GetByFilterAsync(MatchFilter filter);
    public Task<MatchDto> GetByIdAsync(string id);
    public Task<MatchDto> DeleteAsync(string matchId, ClaimsPrincipal user);
    public Task<MatchDto> UpdateAsync(string id, MatchDto newsDto, ClaimsPrincipal user);
    public Task<MatchDto> UpdateForGrpcAsync(string id, MatchDto newsDto, string userId);
    public Task<MatchDto> AddAsync(MatchDto newsDto);
    Task<MatchDto> GetByRoundAsync(string tournamentId, string round);
    Task<bool> AddMatchesAsync(List<MatchDto> matches);
}
