using System;
using System.Text.RegularExpressions;
using AutoMapper;
using TournamentService.BusinessLogic.Models.Filters;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Models.Tournament;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.BusinessLogic.Services.Tournaments;
using TournamentService.BusinessLogic.Services.Tournaments.Interfaces;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.DataAccess.Specifications;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
using TournamentService.Shared.Exceptions;

namespace TournamentService.BusinessLogic.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly ISingleEliminationBracket _singleEliminationBracket;
    private readonly IRoundRobinBracket _roundRobinBracket;
    private readonly ISwissBracket _swissBracket;
    private readonly IMapper _mapper;
    public TournamentService(ITournamentRepository tournamentRepository, 
                             IParticipantRepository participantRepository, 
                             IMapper mapper,
                             ISingleEliminationBracket singleEliminationBracket,
                             IRoundRobinBracket roundRobinBracket,
                             ISwissBracket swissBracket){
        _tournamentRepository = tournamentRepository;
        _participantRepository = participantRepository;
        _mapper = mapper;
        _singleEliminationBracket = singleEliminationBracket;
        _roundRobinBracket = roundRobinBracket;
        _swissBracket = swissBracket;
    }
    public async Task<TournamentDto> AddAsync(TournamentCreateDto newsDto, string ownerId)
    {
        var news = _mapper.Map<Tournament>(newsDto);
        news.OwnerId = ownerId;
        news.Status = TournamentStatus.Pending;
        news.Id = Guid.NewGuid().ToString();
        //news.Id = new Guid().ToString();
        var result = await _tournamentRepository.AddAsync(news);
        return _mapper.Map<TournamentDto>(result);
    }

    public async Task<TournamentDto> DeleteAsync(string id, string userId)
    {
        var obj = await _tournamentRepository.GetByIdAsync(id);
        if(obj == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if(!obj.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var result = _tournamentRepository.DeleteAsync(obj);
        return _mapper.Map<TournamentDto>(result);
    }

    public async Task<List<TournamentCleanDto>> GetAllByPageAsync(int page, int pageSize)
    {
        var list = await _tournamentRepository.GetAsync(page, pageSize);
        return _mapper.Map<List<TournamentCleanDto>>(list);
    }

    public async Task<List<TournamentCleanDto>> GetByFilterAsync(TournamentFilter filter, int page, int pageSize)
    {
        TournamentSpecification spec = TournamentSpecification.FilterTournaments(filter.SearchString, filter.CategoryId, filter.Status, filter.Format, filter.StartTime, filter.EndTime);
        var list = await _tournamentRepository.GetBySpecificationAsync(spec, page, pageSize);
        return _mapper.Map<List<TournamentCleanDto>>(list);
    }

    public async Task<TournamentDto> GetByIdAsync(string id)
    {
        var res = await _tournamentRepository.GetByIdAsync(id);
        if(res == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        return _mapper.Map<TournamentDto>(res);
    }
    public async Task<TournamentDto> UpdateAsync(string id, TournamentDto newsDto, string userId)
    {
        var news = await _tournamentRepository.GetByIdAsync(id);
        if(news == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if(!news.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var newsUp = _mapper.Map(newsDto, news);
        var res = _tournamentRepository.UpdateAsync(newsUp);
        return _mapper.Map<TournamentDto>(res);
    }

    public async void SetNextRound(string id, string userId)
    {
        var news = await _tournamentRepository.GetByIdAsync(id);
        if(news == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if(!news.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        switch (news.Format){
            case TournamentFormat.Swiss:
                await _swissBracket.GenerateSwissMatches(id);
                break;
            default: throw new WrongCallException(ErrorName.WrongTournamentOperationCall);
        }
    }
    public async void SetWinnerForMatchAsync(string tournamentId, string matchId, string winnerId, string looserId, int winPoints, int loosePoints, string userId)
    {
        var news = await _tournamentRepository.GetByIdAsync(tournamentId);
        if(news is null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if(!news.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        switch (news.Format){
            case TournamentFormat.Swiss:
                await _swissBracket.HandleMatchResult(matchId, winnerId, looserId, winPoints, loosePoints);
                break;
            case TournamentFormat.SingleElimination:
                //Console.WriteLine("Получаю винера");
                // var winner = _participantRepository.GetById(winnerId);
                // winner.Points += 1;
                //Console.WriteLine("Обновляю винера");
                // await _participantRepository.UpdateAsync(winner);
                // Console.WriteLine("Обновил винера");
                await _participantRepository.UpdatePointsAsync(winnerId, 1);
                await _singleEliminationBracket.HandleMatchResult(matchId, winnerId, looserId, winPoints, loosePoints);
                break;
            case TournamentFormat.RoundRobin:
                await _roundRobinBracket.HandleMatchResult(matchId, winnerId, winPoints, loosePoints);
                break;
            case TournamentFormat.DoubleElimination:
                throw new NotImplementedException();
            default: throw new WrongCallException(ErrorName.WrongTournamentOperationCall);
        }
    }
    public async Task<TournamentCleanDto> StartTournamentAsync(string tournamentId, string userId)
    {
        var news = await _tournamentRepository.GetByIdAsync(tournamentId);
        if(news == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if(!news.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        news.Status = TournamentStatus.Ongoing;
        GenerateBracketAsync(tournamentId, userId);
        await _tournamentRepository.UpdateAsync(news);
        return _mapper.Map<TournamentCleanDto>(news);
    }
    public async Task<TournamentCleanDto> EndTournamentAsync(string tournamentId, string userId)
    {
        var news = await _tournamentRepository.GetByIdAsync(tournamentId);
        if(news == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if(!news.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        news.Status = TournamentStatus.Completed;
        var participamts = _participantRepository.GetAllAsync(tournamentId);
        participamts = participamts.OrderByDescending(p => p.Points).ToList();
        news.WinnerId = participamts[0].Id;
        await _tournamentRepository.UpdateAsync(news);
        return _mapper.Map<TournamentCleanDto>(news);
    }
    public async void GenerateBracketAsync(string id, string userId)
    {
        var news = await _tournamentRepository.GetByIdAsync(id);
        if(news == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        if(!news.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        switch (news.Format){
            case TournamentFormat.Swiss:
                SetNextRound(id, userId);
                break;
            case TournamentFormat.SingleElimination:
                await _singleEliminationBracket.GenerateBracket(id);
                break;
            case TournamentFormat.RoundRobin:
                await _roundRobinBracket.GenerateBracket(id);
                break;
            case TournamentFormat.DoubleElimination:
                throw new NotImplementedException();
            default: throw new WrongCallException(ErrorName.WrongTournamentOperationCall);
        }
    }
}
