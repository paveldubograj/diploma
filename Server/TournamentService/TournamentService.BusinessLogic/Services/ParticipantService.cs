using System;
using AutoMapper;
using TournamentService.BusinessLogic.Models.ParticipantDtos;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Exceptions;

namespace TournamentService.BusinessLogic.Services;

public class ParticipantService : IParticipantService
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IMapper _mapper;
    public ParticipantService(IParticipantRepository participantRepository, IMapper mapper){
        _participantRepository = participantRepository;
        _mapper = mapper;
    }
    public async Task<ParticipantDto> AddAsync(ParticipantAddDto newsDto, string tournamentId)
    {
        var news = _mapper.Map<Participant>(newsDto);
        news.Id = Guid.NewGuid().ToString();
        news.TournamentId = tournamentId;
        news.Status = Shared.Enums.ParticipantStatus.Play;
        var result = await _participantRepository.AddAsync(news);
        return _mapper.Map<ParticipantDto>(result);
    }

    public async Task<ParticipantDto> DeleteAsync(string id, string userId)
    {
        var obj = await _participantRepository.GetByIdAsync(id);
        if(obj == null){
            throw new NotFoundException(ErrorName.ParticipantNotFound);
        }
        if(!obj.Tournament.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var result = _participantRepository.DeleteAsync(obj);
        return _mapper.Map<ParticipantDto>(result);
    }

    public async Task<List<ParticipantDto>> GetAllByPageAsync(string tournamentId, int page, int pageSize)
    {
        var list = await _participantRepository.GetAsync(tournamentId, page, pageSize);
        return _mapper.Map<List<ParticipantDto>>(list);
    }

    public async Task<List<ParticipantDto>> GetAllByTournamentAsync(string tournamentId)
    {
        var list = _participantRepository.GetAllAsync(tournamentId);
        return _mapper.Map<List<ParticipantDto>>(list);
    }

    public async Task<ParticipantDto> GetByIdAsync(string id)
    {
        var res = await _participantRepository.GetByIdAsync(id);
        if(res == null){
            throw new NotFoundException(ErrorName.ParticipantNotFound);
        }
        return _mapper.Map<ParticipantDto>(res);
    }

    public async Task<ParticipantDto> UpdateAsync(string id, ParticipantDto newsDto, string userId)
    {
        var news = await _participantRepository.GetByIdAsync(id);
        if(news == null){
            throw new NotFoundException(ErrorName.ParticipantNotFound);
        }
        if(!news.Tournament.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var newsUp = _mapper.Map(newsDto, news);
        var res = _participantRepository.UpdateAsync(newsUp);
        return _mapper.Map<ParticipantDto>(res);
    }

    public async Task<ParticipantDto> UpdatePointsAsync(string id, int points, string userId)
    {
        //Thread.Sleep(3000);
        //var news = _participantRepository.GetById(id);
        // if(news == null){
        //     throw new NotFoundException(ErrorName.ParticipantNotFound);
        // }
        // if(!news.Tournament.OwnerId.Equals(userId)){
        //     throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        // }
        // news.Points += points;
        var res = await _participantRepository.UpdatePointsAsync(id, points);
        return _mapper.Map<ParticipantDto>(res);
    }
}
