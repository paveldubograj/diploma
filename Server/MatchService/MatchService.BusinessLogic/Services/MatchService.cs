using System;
using System.Security.Claims;
using System.Xml.Serialization;
using AutoMapper;
using MatchService.BusinessLogic.Models.Filter;
using MatchService.BusinessLogic.Models.Match;
using MatchService.BusinessLogic.Services.Interfaces;
using MatchService.DataAccess.Entities;
using MatchService.DataAccess.Repositories;
using MatchService.DataAccess.Repositories.Interfaces;
using MatchService.DataAccess.Specifications;
using MatchService.Shared.Constants;
using MatchService.Shared.Enums;
using MatchService.Shared.Exceptions;
using Newtonsoft.Json;

namespace MatchService.BusinessLogic.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    public MatchService(IMatchRepository matchRepository, IMapper mapper, ICacheService cacheService)
    {
        _matchRepository = matchRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }
    public async Task<MatchDto> AddAsync(MatchDto matchDto)
    {
        var news = _mapper.Map<Match>(matchDto);
        var result = await _matchRepository.AddAsync(news);
        return _mapper.Map<MatchDto>(result);
    }
    public async Task<MatchDto> DeleteAsync(string matchId, ClaimsPrincipal user)
    {
        var obj = await _matchRepository.GetByIdAsync(matchId);
        if(obj == null){
            throw new NotFoundException(ErrorName.MatchNotFound);
        }
        if(!obj.OwnerId.Equals(user.Claims.First(c => c.Type.Equals(ClaimTypes.Name)).Value) && !user.IsInRole(RoleName.Admin)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var result = await _matchRepository.DeleteAsync(obj);
        return _mapper.Map<MatchDto>(result);
    }
    public async Task<List<MatchListDto>> GetAllByPageAsync(int page, int pageSize)
    {
        var list = await _matchRepository.GetAsync(page, pageSize);
        return _mapper.Map<List<MatchListDto>>(list);
    }
    public async Task<MatchPagedResponse> GetByFilterAsync(MatchFilter filter)
    {
        var serializedValue = JsonConvert.SerializeObject(filter);
        var cache = await _cacheService.GetAsync<MatchPagedResponse>(serializedValue);
        if (cache != null) return cache;
        MatchSpecification specification = MatchSpecification.FilterMatch(filter.CategoryId, filter.StartTime, filter.EndTime, filter.TournamentId, filter.Status);
        var list = await _matchRepository.GetBySpecificationAsync(specification, filter.Options, filter.Page, filter.PageSize);
        var result = _mapper.Map<MatchPagedResponse>(list);
        await _cacheService.SetAsync<MatchPagedResponse>(serializedValue, result);
        return result;
    }
    public async Task<MatchDto> GetByIdAsync(string id)
    {
        var res = await _matchRepository.GetByIdAsync(id);
        if(res == null){
            throw new NotFoundException(ErrorName.MatchNotFound);
        }
        return _mapper.Map<MatchDto>(res);
    }
    public async Task<List<MatchListDto>> GetTournamentStructureAsync(string id)
    {
        var res = await _matchRepository.GetTournamentStructureAsync(id);
        if(res == null){
            throw new NotFoundException(ErrorName.MatchNotFound);
        }
        return _mapper.Map<List<Match>, List<MatchListDto>>(res);
    }
    public async Task<MatchDto> UpdateAsync(string id, MatchDto newsDto, ClaimsPrincipal user)
    {
        var obj = await _matchRepository.GetByIdAsync(id);
        if(obj == null){
            throw new NotFoundException(ErrorName.MatchNotFound);
        }
        if(!obj.OwnerId.Equals(user.Claims.First(c => c.Type.Equals(ClaimTypes.Name)).Value) || !user.IsInRole(RoleName.Admin)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var newsUp = _mapper.Map(newsDto, obj);
        var res = await _matchRepository.UpdateAsync(newsUp);
        return _mapper.Map<MatchDto>(res);
    }
    public async Task<MatchDto> UpdateForGrpcAsync(string id, MatchDto newsDto, string userId)
    {
        var obj = await _matchRepository.GetByIdAsync(id);
        if(obj == null){
            throw new NotFoundException(ErrorName.MatchNotFound);
        }
        if(!obj.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var newsUp = _mapper.Map(newsDto, obj);
        var res = await _matchRepository.UpdateAsync(newsUp);
        return _mapper.Map<MatchDto>(res);
    }
    public async Task<MatchDto> GetByRoundAsync(string tournamentId, string round){
        MatchSpecification specification = MatchSpecification.FindTournamentRound(tournamentId, round);
        var result = await _matchRepository.GetOneBySpecificationAsync(specification);
        return _mapper.Map<MatchDto>(result);
    }
    public async Task<bool> AddMatchesAsync(List<MatchDto> matches){
        List<Match> matches1;
        matches1 = _mapper.Map<List<Match>>(matches);
        if(matches1.Count < 1) throw new ArgumentException(ErrorName.EmptyMatchList);
        await _matchRepository.AddRange(matches1);
        return true;
    }
}
