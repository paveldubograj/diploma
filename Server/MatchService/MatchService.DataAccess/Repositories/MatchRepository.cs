using System;
using MatchService.DataAccess.Database;
using MatchService.DataAccess.Entities;
using MatchService.DataAccess.Repositories.Interfaces;
using MatchService.DataAccess.Specifications;
using MatchService.DataAccess.Specifications.SpecSettings;
using Microsoft.EntityFrameworkCore;

namespace MatchService.DataAccess.Repositories;

public class MatchRepository : IMatchRepository
{
    private MatchContext db;

    public MatchRepository(MatchContext db)
    {
        this.db = db;
    }
    public async Task<Match> AddAsync(Match Match)
    {
        db.Entry(Match).State = EntityState.Added;
        await db.SaveChangesAsync();
        return Match;
    }
    public async Task<Match> DeleteAsync(Match Match)
    {
        var removedEntity = db.Set<Match>().Remove(Match).Entity;
        await db.SaveChangesAsync();
        return removedEntity;
    }
    public async Task<List<Match>> GetAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;

        var query = await db.Matches
            .OrderByDescending(n => n.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return query;
    }
    public async Task<Match> GetByIdAsync(string id)
    {
        var entity = await db.Matches.Where(t => t.Id.Equals(id)).FirstOrDefaultAsync();
        return entity;
    }
    public async Task<IEnumerable<Match>> GetBySpecificationAsync(MatchSpecification spec, int page, int pageSize, CancellationToken token = default)
    {
        IQueryable<Match> query = db.Matches.OrderByDescending(n => n.StartTime);

        query = query.ApplySpecification(spec).Skip((page - 1) * pageSize).Take(pageSize);

        return await query.ToListAsync(cancellationToken: token);
    }
    public async Task<List<Match>> GetTournamentStructureAsync(string tournamentId)
    {
        var matches = await db.Matches
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.Round)
            .ThenBy(m => m.MatchOrder)
            .ToListAsync();
        return matches;
    }
    public async Task<Match> UpdateAsync(Match Match)
    {
        db.Entry(Match).State = EntityState.Modified;
        return Match;
    }
}
