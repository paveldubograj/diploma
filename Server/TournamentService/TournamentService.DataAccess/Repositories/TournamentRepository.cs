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
    private readonly TournamentContext _context;
    public TournamentRepository(TournamentContext context){
        _context = context;
    }
    public async Task<Tournament> AddAsync(Tournament tournament)
    {
        _context.Set<Tournament>().Add(tournament);
        await _context.SaveChangesAsync();
        return tournament;
    }

    public async Task<Tournament> DeleteAsync(Tournament tournament)
    {
        var removedEntity = _context.Set<Tournament>().Remove(tournament).Entity;
        await _context.SaveChangesAsync();
        return removedEntity;
    }

    public async Task<List<Tournament>> GetAsync(int page, int pageSize)
    {
        return await _context.Tournaments
            .OrderByDescending(n => n.StartDate)
            //.Skip((page - 1) * pageSize)
            //.Take(pageSize)
            .ToListAsync();
    }

    public async Task<Tournament> GetByIdAsync(string id)
    {
        return _context.Tournaments.Find(id);
            //.Include(t => t.Participants)
            //.FirstOrDefaultAsync(t => t.Id.Equals(id));
    }

    public async Task<IEnumerable<Tournament>> GetBySpecificationAsync(TournamentSpecification spec1, int page, int pageSize, CancellationToken token = default)
    {
        IQueryable<Tournament> query = _context.Tournaments.OrderByDescending(n => n.StartDate);

        query = query.ApplySpecification(spec1).Skip((page - 1) * pageSize).Take(pageSize);

        return await query.ToListAsync(cancellationToken: token);
    }

    public async Task<Tournament> UpdateAsync(Tournament tournament)
    {
        _context.Entry(tournament).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return tournament;
    }
}
