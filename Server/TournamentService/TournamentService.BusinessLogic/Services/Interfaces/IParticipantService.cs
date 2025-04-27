using System;
using TournamentService.BusinessLogic.Models.ParticipantDtos;

namespace TournamentService.BusinessLogic.Services.Interfaces;

public interface IParticipantService
{
    public Task<List<ParticipantDto>> GetAllByPageAsync(string tournamentId, int page, int pageSize);
    public Task<ParticipantDto> GetByIdAsync(string id);
    public Task<ParticipantDto> DeleteAsync(string id, string userId);
    public Task<ParticipantDto> UpdateAsync(string id, ParticipantDto newsDto, string userId);
    public Task<ParticipantDto> AddAsync(ParticipantAddDto newsDto, string tournamentId);
    public Task<List<ParticipantDto>> GetAllByTournamentAsync(string tournamentId);
    public Task<ParticipantDto> UpdatePointsAsync(string id, int points, string userId);
}
