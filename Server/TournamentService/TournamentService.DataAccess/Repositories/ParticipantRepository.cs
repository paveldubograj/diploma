using System;
using Microsoft.EntityFrameworkCore;
using TournamentService.DataAccess.Database;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Exceptions;

namespace TournamentService.DataAccess.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private readonly TournamentContext _context;
    public ParticipantRepository(TournamentContext context){
        _context = context;
    }
    public async Task<Participant> AddAsync(Participant participant)
    {
        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();
        return participant;
    }

    public async Task<Participant> AddParticipantToTournament(string tournamentId, string participantId)
    {
        var tournament = await _context.Tournaments.FindAsync(tournamentId);
        var participant = await _context.Participants.FindAsync(participantId);
        
        if (tournament == null || participant == null) throw new NotFoundException();

        tournament.Participants.Add(participant);
        await _context.SaveChangesAsync();
        return await _context.Participants.FindAsync(participantId);
    }

    public async Task<Participant> DeleteAsync(Participant participant)
    {
        var removedEntity = _context.Set<Participant>().Remove(participant).Entity;
        await _context.SaveChangesAsync();
        return removedEntity;
    }

    public async Task<List<Participant>> GetAsync(string tournamentId, int page, int pageSize)
    {
        return await _context.Participants
            .Where(c => c.TournamentId.Equals(tournamentId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Participant>> GetAllAsync(string tournamentId)
    {
        return await _context.Participants
            .Where(c => c.TournamentId.Equals(tournamentId))
            .ToListAsync();
    }

    public async Task<Participant> GetByIdAsync(string id)
    {
        return await _context.Participants.Include(c => c.Tournament)
        .FirstOrDefaultAsync(t => t.Id.Equals(id));
    }

    public async Task<Participant> RemoveParticipantFromTournament(string tournamentId, string participantId)
    {
        var tournament = await _context.Tournaments.FindAsync(tournamentId);
        var participant = await _context.Participants.FindAsync(participantId);
        
        if (tournament == null || participant == null) throw new NotFoundException();

        tournament.Participants.Remove(participant);
        await _context.SaveChangesAsync();
        return participant;
    }

    public async Task<Participant> UpdateAsync(Participant participant)
    {
        _context.Entry(participant).State = EntityState.Modified;
        return participant;
    }
}
