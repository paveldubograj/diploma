using System;
using AutoMapper;
using TournamentService.BusinessLogic.Models.ParticipantDtos;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
using TournamentService.Shared.Exceptions;

namespace TournamentService.BusinessLogic.Services;

public class ParticipantService : IParticipantService
{
    private readonly IParticipantRepository _participantRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUserGrpcService _userGrpcService;
    private readonly IMapper _mapper;
    public ParticipantService(IParticipantRepository participantRepository, ITournamentRepository tournamentRepository, IUserGrpcService userGrpcService, IMapper mapper){
        _participantRepository = participantRepository;
        _tournamentRepository = tournamentRepository;
        _userGrpcService = userGrpcService;
        _mapper = mapper;
    }
    public async Task<ParticipantDto> AddAsync(ParticipantAddDto newsDto, string tournamentId)
    {
        var participant = _mapper.Map<Participant>(newsDto);
        participant.Id = Guid.NewGuid().ToString();
        participant.TournamentId = tournamentId;
        participant.Status = ParticipantStatus.PlayWin;
        var result = await _participantRepository.AddAsync(participant);
        return _mapper.Map<ParticipantDto>(result);
    }

    public async Task<ParticipantDto> RegisterAsync(RegisterForTournamentDto registerDto, string tournamentId)
    {
        if(!await _tournamentRepository.IsRegistrationAllowed(tournamentId)) throw new WrongCallException(ErrorName.RegistrationNotAllowed);
        var participant = _mapper.Map<Participant>(registerDto);
        participant.Id = Guid.NewGuid().ToString();
        participant.TournamentId = tournamentId;
        participant.Status = ParticipantStatus.PlayWin;
        var result = await _participantRepository.AddAsync(participant);
        await _userGrpcService.AddToUser(registerDto.UserId, tournamentId);
        return _mapper.Map<ParticipantDto>(result);
    }

    public async Task<ParticipantDto> DeleteAsync(string id, string userId)
    {
        var participant = await _participantRepository.GetByIdAsync(id);
        if(participant == null){
            throw new NotFoundException(ErrorName.ParticipantNotFound);
        }
        if(!participant.Tournament.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var result = await _participantRepository.RemoveParticipantFromTournament(participant.TournamentId, participant.Id);
        if(!string.IsNullOrEmpty(participant.UserId)) await _userGrpcService.RemoveFromUser(participant.UserId, participant.TournamentId);
        return _mapper.Map<ParticipantDto>(result);
    }

    public async Task<List<ParticipantDto>> GetAllByPageAsync(string tournamentId, ParticipantSortOptions? options, int page, int pageSize)
    {
        var list = await _participantRepository.GetAsync(tournamentId, options, page, pageSize);
        return _mapper.Map<List<ParticipantDto>>(list);
    }

    public async Task<List<ParticipantDto>> GetAllByTournamentAsync(string tournamentId)
    {
        var list = await _participantRepository.GetAllAsync(tournamentId);
        return _mapper.Map<List<ParticipantDto>>(list);
    }

    public async Task<List<ParticipantSListDto>> GetPlayingByTournamentAsync(string tournamentId)
    {
        var list = await _participantRepository.GetAllPlayingAsync(tournamentId);
        return _mapper.Map<List<ParticipantSListDto>>(list);
    }

    public async Task<ParticipantDto> GetByIdAsync(string id)
    {
        var participant = await _participantRepository.GetByIdAsync(id);
        if(participant == null){
            throw new NotFoundException(ErrorName.ParticipantNotFound);
        }
        return _mapper.Map<ParticipantDto>(participant);
    }

    public async Task<ParticipantDto> UpdateAsync(string id, ParticipantDto newsDto, string userId)
    {
        var participant = await _participantRepository.GetByIdWithToutnamentAsync(id);
        if(participant == null){
            throw new NotFoundException(ErrorName.ParticipantNotFound);
        }
        if(!participant.Tournament.OwnerId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var newsUp = _mapper.Map(newsDto, participant);
        var res = await _participantRepository.UpdateAsync(newsUp);
        return _mapper.Map<ParticipantDto>(res);
    }

    public async Task<List<ParticipantDto>> GetAllFromLowerAsync(string tournamentId)
    {
        return _mapper.Map<List<ParticipantDto>>(await _participantRepository.GetAllFromLowerAsync(tournamentId));
    }

    public async Task<List<ParticipantDto>> GetAllFromUpperAsync(string tournamentId)
    {
        return _mapper.Map<List<ParticipantDto>>(await _participantRepository.GetAllFromUpperAsync(tournamentId));
    }

    public async Task<ParticipantDto> UpdatePointsAsync(string id, int points)
    {
        var participant = await _participantRepository.UpdatePointsAsync(id, points);
        return _mapper.Map<ParticipantDto>(participant);
    }
}
