using System;
using Microsoft.EntityFrameworkCore;
using TournamentService.DataAccess.Database;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
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
        if((await GetAllAsync(tournament.Id)).Count >= tournament.MaxParticipants) throw new WrongCallException(ErrorName.MaxParticipants);
        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();
        return participant;
    }

    public async Task<Participant> AddParticipantToTournament(string tournamentId, string participantId)
    {
        var tournament = await _context.Tournaments.Include(p => p.Participants).Where(t => t.Id.Equals(tournamentId)).FirstOrDefaultAsync();
        var participant = await _context.Participants.FindAsync(participantId);
        
        if (tournament == null) throw new NotFoundException("Tournament not found");
        if((await GetAllAsync(tournament.Id)).Count >= tournament.MaxParticipants) throw new WrongCallException(ErrorName.MaxParticipants);
        if (participant == null) throw new NotFoundException("Participant not found");

        tournament.Participants.Add(participant);
        await _context.SaveChangesAsync();
        return participant;
    }

    public async Task<Participant> DeleteAsync(Participant participant)
    {
        var removedEntity = _context.Set<Participant>().Remove(participant).Entity;
        await _context.SaveChangesAsync();
        return removedEntity;
    }

    public async Task<List<Participant>> GetAsync(string tournamentId, ParticipantSortOptions? options, int page, int pageSize)
    {
        var query = _context.Participants
            .Where(c => c.TournamentId.Equals(tournamentId))
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        switch (options){
            case ParticipantSortOptions.ByName:
                query = query.OrderBy(c => c.Name);
                break;
            case ParticipantSortOptions.ByNameDesc:
                query = query.OrderByDescending(c => c.Name);
                break;
            case ParticipantSortOptions.ByPoints:
                query = query.OrderBy(c => c.Points);
                break;
            case ParticipantSortOptions.ByPointsdesc:
                query = query.OrderByDescending(c => c.Points);
                break;
            default: 
                query = query.OrderBy(c => c.Name);
                break;
        }
        return await query.ToListAsync();
    }

    public async Task<List<Participant>> GetAllAsync(string tournamentId)
    {
        return await _context.Participants.Where(c => c.TournamentId.Equals(tournamentId)).ToListAsync();
    }

    public async Task<List<Participant>> GetAllPlayingAsync(string tournamentId)
    {
        return await _context.Participants.Where(c => c.TournamentId.Equals(tournamentId) && (c.Status == ParticipantStatus.PlayLoose || c.Status == ParticipantStatus.PlayWin)).ToListAsync();
    }

    public async Task<List<Participant>> GetAllFromLowerAsync(string tournamentId)
    {
        return await _context.Participants.Where(c => c.TournamentId.Equals(tournamentId) && c.Status == ParticipantStatus.PlayLoose).AsNoTracking().ToListAsync();
    }

    public async Task<List<Participant>> GetAllFromUpperAsync(string tournamentId)
    {
        return await _context.Participants.Where(c => c.TournamentId.Equals(tournamentId) && c.Status == ParticipantStatus.PlayWin).AsNoTracking().ToListAsync();
    }

    public async Task<Participant?> GetByIdAsync(string id)
    {
        return await _context.Set<Participant>().FindAsync(id);
    }

    public async Task<Participant> GetByIdWithToutnamentAsync(string id)
    {
        return await _context.Set<Participant>().Include(c => c.Tournament).FirstAsync(c => c.Id.Equals(id));
    }
    public async Task<Participant> RemoveParticipantFromTournament(string tournamentId, string participantId)
    {
        var tournament = await _context.Tournaments.FindAsync(tournamentId);
        var participant = await _context.Participants.FindAsync(participantId);
        
        if (tournament == null || participant == null) throw new NotFoundException();

        tournament.Participants.Remove(participant);
        _context.Participants.Remove(participant);
        await _context.SaveChangesAsync();
        return participant;
    }

    public async Task<Participant> UpdateAsync(Participant participant)
    {
        _context.Entry(participant).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return participant;
    }

    public async Task<Participant> UpdatePointsAsync(string id, int points)
    {
        var p = await _context.Set<Participant>().FindAsync(id);
        if (p is null)
        {
            throw new NotFoundException(ErrorName.ParticipantNotFound);
        }
        p.Points += points;
        _context.Entry(p).Property(p => p.Points).IsModified = true;
        await _context.SaveChangesAsync();
        return p;
    }
}
