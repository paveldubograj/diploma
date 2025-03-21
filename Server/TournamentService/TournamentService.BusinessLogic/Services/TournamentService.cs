using System;
using AutoMapper;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Models.Tournament;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Exceptions;

namespace TournamentService.BusinessLogic.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IMapper _mapper;
    public TournamentService(ITournamentRepository tournamentRepository, IParticipantRepository participantRepository, IMapper mapper){
        _tournamentRepository = tournamentRepository;
        _participantRepository = participantRepository;
        _mapper = mapper;
    }
    public async Task<TournamentDto> AddAsync(TournamentDto newsDto)
    {
        var news = _mapper.Map<Tournament>(newsDto);
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

    public async Task<TournamentDto> GetByIdAsync(string id)
    {
        var res = await _tournamentRepository.GetByIdAsync(id);
        if(res == null){
            throw new NotFoundException(ErrorName.TournamentNotFound);
        }
        return _mapper.Map<TournamentDto>(res);
    }

    public Task<List<MatchDto>> SetNextRound(string tournamentId)
    {
        throw new NotImplementedException();
    }

    public Task<MatchDto> SetWinnerForMatchAsync(string tournamentId, string matchId, string participantId, int winPoints, int loosePoints)
    {
        throw new NotImplementedException();
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
}
