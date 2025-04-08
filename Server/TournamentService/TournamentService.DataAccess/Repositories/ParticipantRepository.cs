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
        var tournament = await _context.Tournaments.FindAsync(participant.TournamentId);
        if (tournament == null) throw new NotFoundException("Tournament not found");
        if(GetAllAsync(tournament.Id).Count >= tournament.MaxParticipants) throw new Exception("There are already max participants in tournament!");
        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();
        return participant;
    }

    public async Task<Participant> AddParticipantToTournament(string tournamentId, string participantId)
    {
        var tournament = await _context.Tournaments.Include(p => p.Participants).Where(t => t.Id.Equals(tournamentId)).FirstOrDefaultAsync();
        var participant = await _context.Participants.FindAsync(participantId);
        
        if (tournament == null) throw new NotFoundException("Tournament not found");
        if (participant == null) throw new NotFoundException("Participant not found");

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
            // .Skip((page - 1) * pageSize)
            // .Take(pageSize)
            .ToListAsync();
    }

    public List<Participant> GetAllAsync(string tournamentId)
    {
        return _context.Participants.Where(c => c.TournamentId.Equals(tournamentId)).ToList();
    }

    public async Task<Participant> GetByIdAsync(string id)
    {
        return await _context.Participants
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
