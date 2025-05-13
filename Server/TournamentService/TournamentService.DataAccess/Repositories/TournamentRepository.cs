using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TournamentService.DataAccess.Database;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.DataAccess.Specifications;
using TournamentService.DataAccess.Specifications.SpecSettings;
using TournamentService.Shared.Enums;

namespace TournamentService.DataAccess.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly TournamentContext _db;
    public TournamentRepository(TournamentContext context){
        _db = context;
    }
    public async Task<Tournament> AddAsync(Tournament tournament)
    {
        _db.Set<Tournament>().Add(tournament);
        _db.SaveChanges();
        return tournament;
    }

    public async Task<Tournament> DeleteAsync(Tournament tournament)
    {
        var removedEntity = _db.Set<Tournament>().Remove(tournament).Entity;
        await _db.SaveChangesAsync();
        return removedEntity;
    }

    public async Task<List<Tournament>> GetAsync(int page, int pageSize)
    {
        return await _db.Tournaments
            .OrderByDescending(n => n.StartDate)
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Tournament?> GetByIdAsync(string id)
    {
        var r = await _db.Tournaments.FindAsync(id);
        //_context.Dispose();
        return r;
            //.Include(t => t.Participants)
            //.FirstOrDefaultAsync(t => t.Id.Equals(id));
    }

    public Tournament GetById(string id)
    {
        return _db.Tournaments.Find(id);
            //.Include(t => t.Participants)
            //.FirstOrDefaultAsync(t => t.Id.Equals(id));
    }

    public async Task<IEnumerable<Tournament>> GetBySpecificationAsync(TournamentSpecification spec1, TournamentSortOptions? options, int page, int pageSize, CancellationToken token = default)
    {
        IQueryable<Tournament> query = _db.Tournaments
            .OrderByDescending(n => n.StartDate)
            .AsNoTracking()
            .ApplySpecification(spec1)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        switch (options){
            case TournamentSortOptions.ByName:
                query = query.OrderBy(c => c.Name);
                break;
            case TournamentSortOptions.ByNameDesc:
                query = query.OrderByDescending(c => c.Name);
                break;
            case TournamentSortOptions.ByDate:
                query = query.OrderBy(c => c.StartDate);
                break;
            case TournamentSortOptions.ByDateDesc:
                query = query.OrderByDescending(c => c.StartDate);
                break;
            case TournamentSortOptions.ByParticipants:
                query = query.OrderBy(c => c.MaxParticipants);
                break;
            case TournamentSortOptions.ByParticipantsDesc:
                query = query.OrderByDescending(c => c.MaxParticipants);
                break;
            default:
                query = query.OrderBy(c => c.Name);
                break;
        }

        return await query.ToListAsync(cancellationToken: token);
    }

    public async Task<Tournament> UpdateAsync(Tournament tournament)
    {
        _db.Entry(tournament).State = EntityState.Modified;
        _db.SaveChanges();
        return tournament;
    }

    public Participant GetParticipantById(string id)
    {
        return _db.Participants.Find(id);
       // .FirstOrDefaultAsync(t => t.Id.Equals(id));
    }

    public async Task<int> GetTotalAsync(){
        return await _db.Participants.CountAsync();
    }

    public async Task<bool> IsRegistrationAllowed(string tournamentId){
        return (await _db.Tournaments.FindAsync(tournamentId)).IsRegistrationAllowed;
    }
}
