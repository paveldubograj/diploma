using System;
using TournamentService.DataAccess.Entities;

namespace TournamentService.DataAccess.Repositories.Interfaces;

public interface IParticipantRepository
{
    Task<Participant> GetByIdAsync(string id);
    public Participant GetById(string id);
    Task<List<Participant>> GetAsync(string tournamentId, int page, int pageSize);
    public List<Participant> GetAllAsync(string tournamentId);
    Task<Participant> AddAsync(Participant participant);
    Task<Participant> DeleteAsync(Participant participant);
    Task<Participant> UpdateAsync(Participant participant);
    public Participant Update(Participant participant);
    public Task<Participant> UpdatePointsAsync(string id, int points);
    public Task<Participant> AddParticipantToTournament(string tournamentId, string participantId);
    public Task<Participant> RemoveParticipantFromTournament(string tournamentId, string participantId);
}
