using System;
using TournamentService.BusinessLogic.Models.ParticipantDtos;
using TournamentService.Shared.Enums;

namespace TournamentService.BusinessLogic.Services.Interfaces;

public interface IParticipantService
{
    public Task<List<ParticipantDto>> GetAllByPageAsync(string tournamentId, ParticipantSortOptions? options, int page, int pageSize);
    public Task<ParticipantDto> GetByIdAsync(string id);
    public Task<ParticipantDto> DeleteAsync(string id, string userId);
    public Task<ParticipantDto> UpdateAsync(string id, ParticipantDto newsDto, string userId);
    public Task<ParticipantDto> AddAsync(ParticipantAddDto newsDto, string tournamentId);
    public Task<ParticipantDto> RegisterAsync(RegisterForTournamentDto registerDto, string tournamentId);
    public Task<List<ParticipantDto>> GetAllByTournamentAsync(string tournamentId);
    public Task<List<ParticipantDto>> GetAllFromLowerAsync(string tournamentId);
    public Task<List<ParticipantDto>> GetAllFromUpperAsync(string tournamentId);
    public Task<List<ParticipantSListDto>> GetPlayingByTournamentAsync(string tournamentId);
    public Task<ParticipantDto> UpdatePointsAsync(string id, int points);
}
