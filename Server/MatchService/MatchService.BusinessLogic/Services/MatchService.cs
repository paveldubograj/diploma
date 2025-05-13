using System;
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

namespace MatchService.BusinessLogic.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMapper _mapper;
    public MatchService(IMatchRepository matchRepository, IMapper mapper){
        _matchRepository = matchRepository;
        _mapper = mapper;
    }
    public async Task<MatchDto> AddAsync(MatchDto matchDto)
    {
        var news = _mapper.Map<Match>(matchDto);
        var result = await _matchRepository.AddAsync(news);
        return _mapper.Map<MatchDto>(result);
    }
    public async Task<MatchDto> DeleteAsync(string matchId, string userId)
    {
        var obj = await _matchRepository.GetByIdAsync(matchId);
        if(obj == null){
            throw new NotFoundException(ErrorName.MatchNotFound);
        }
        if(!obj.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var result = _matchRepository.DeleteAsync(obj);
        return _mapper.Map<MatchDto>(result);
    }
    public async Task<List<MatchListDto>> GetAllByPageAsync(int page, int pageSize)
    {
        var list = await _matchRepository.GetAsync(page, pageSize);
        return _mapper.Map<List<MatchListDto>>(list);
    }
    public async Task<List<MatchListDto>> GetByFilterAsync(MatchFilter filter, SortOptions? options, int page, int pageSize)
    {
        MatchSpecification specification = MatchSpecification.FilterMatch(filter.CategoryId, filter.StartTime, filter.EndTime, filter.TournamentId, filter.Status);
        var result = await _matchRepository.GetBySpecificationAsync(specification, options, page, pageSize);
        return _mapper.Map<List<MatchListDto>>(result);
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
    public async Task<MatchDto> UpdateAsync(string id, MatchDto newsDto, string userId)
    {
        var news = await _matchRepository.GetByIdAsync(id);
        if(news == null){
            throw new NotFoundException(ErrorName.MatchNotFound);
        }
        if(!news.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var newsUp = _mapper.Map(newsDto, news);
        var res = await _matchRepository.UpdateAsync(newsUp);
        return _mapper.Map<MatchDto>(res);
    }
    public async Task<MatchDto> UpdateForUserAsync(string id, MatchUpdateDto newsDto, string userId)
    {
        var news = await _matchRepository.GetByIdAsync(id);
        if(news == null){
            throw new NotFoundException(ErrorName.MatchNotFound);
        }
        if(!news.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var newsUp = _mapper.Map(newsDto, news);
        var res = await _matchRepository.UpdateAsync(newsUp);
        return _mapper.Map<MatchDto>(res);
    }
    public async Task<MatchDto> SetWinnerAsync(string matchId, string winnerId, int winScore, int looseScore, string userId){
        var news = await _matchRepository.GetByIdAsync(matchId);
        if(news == null){
            throw new NotFoundException(ErrorName.MatchNotFound);
        }
        if(!news.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        news.WinnerId = winnerId;
        news.LooseScore = looseScore;
        news.WinScore = winScore;
        var res = _matchRepository.UpdateAsync(news);
        return _mapper.Map<MatchDto>(res);
    }
    public async Task<MatchDto> GetByRoundAsync(string tournamentId, string round){
        MatchSpecification specification = MatchSpecification.FindTournamentRound(tournamentId, round);
        var result = await _matchRepository.GetOneBySpecificationAsync(specification);
        return _mapper.Map<MatchDto>(result);
    }
    public async Task<bool> AddMatchesAsync(List<MatchDto> matches){
        List<Match> matches1 = new List<Match>();
        matches1 = _mapper.Map<List<MatchDto>, List<Match>>(matches);
        if(matches1.Count < 1) throw new ArgumentException(ErrorName.EmptyMatchList);
        await _matchRepository.AddRange(matches1);
        return true;
    }
    public async Task<int> GetTotalAsync(){
        return await _matchRepository.GetTotalAsync();
    }
}
