using System;
using TournamentService.DataAccess.Entities;

namespace TournamentService.DataAccess.Repositories.Interfaces;

public interface IParticipantRepository
{
    Task<Participant> GetByIdAsync(string id);
    Task<List<Participant>> GetAsync(string tournamentId, int page, int pageSize);
    public Task<List<Participant>> GetAllAsync(string tournamentId);
    Task<Participant> AddAsync(Participant participant);
    Task<Participant> DeleteAsync(Participant participant);
    Task<Participant> UpdateAsync(Participant participant);
    public Task<Participant> AddParticipantToTournament(string tournamentId, string participantId);
    public Task<Participant> RemoveParticipantFromTournament(string tournamentId, string participantId);
}
