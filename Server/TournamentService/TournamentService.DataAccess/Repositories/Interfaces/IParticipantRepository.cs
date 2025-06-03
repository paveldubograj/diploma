using System;
using TournamentService.DataAccess.Entities;
using TournamentService.Shared.Enums;

namespace TournamentService.DataAccess.Repositories.Interfaces;

public interface IParticipantRepository
{
    Task<Participant?> GetByIdAsync(string id);
    Task<Participant> GetByIdWithToutnamentAsync(string id);
    Task<List<Participant>> GetAsync(string tournamentId, ParticipantSortOptions? options, int page, int pageSize);
    Task<List<Participant>> GetAllAsync(string tournamentId);
    Task<List<Participant>> GetAllPlayingAsync(string tournamentId);
    Task<List<Participant>> GetAllFromLowerAsync(string tournamentId);
    Task<List<Participant>> GetAllFromUpperAsync(string tournamentId);
    Task<Participant> AddAsync(Participant participant);
    Task<Participant> DeleteAsync(Participant participant);
    Task<Participant> UpdateAsync(Participant participant);
    Task<Participant> UpdatePointsAsync(string id, int points);
    Task<Participant> AddParticipantToTournament(string tournamentId, string participantId);
    Task<Participant> RemoveParticipantFromTournament(string tournamentId, string participantId);
}
