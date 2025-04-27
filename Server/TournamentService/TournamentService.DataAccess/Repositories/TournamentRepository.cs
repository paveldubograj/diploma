using System;
using Microsoft.EntityFrameworkCore;
using TournamentService.DataAccess.Database;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.DataAccess.Specifications;
using TournamentService.DataAccess.Specifications.SpecSettings;

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
        _db.SaveChanges();
        return removedEntity;
    }

    public async Task<List<Tournament>> GetAsync(int page, int pageSize)
    {
        return await _db.Tournaments
            .OrderByDescending(n => n.StartDate)
            //.Skip((page - 1) * pageSize)
            //.Take(pageSize)
            .ToListAsync();
    }

    public async Task<Tournament> GetByIdAsync(string id)
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

    public async Task<IEnumerable<Tournament>> GetBySpecificationAsync(TournamentSpecification spec1, int page, int pageSize, CancellationToken token = default)
    {
        IQueryable<Tournament> query = _db.Tournaments.OrderByDescending(n => n.StartDate);

        query = query.ApplySpecification(spec1).Skip((page - 1) * pageSize).Take(pageSize);

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
}
