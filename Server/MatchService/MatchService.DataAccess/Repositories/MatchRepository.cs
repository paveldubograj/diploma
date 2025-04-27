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
    private readonly MatchContext _context;

    public MatchRepository(MatchContext context)
    {
        _context = context;
    }
    public async Task<Match> AddAsync(Match Match)
    {
        _context.Entry(Match).State = EntityState.Added;
        await _context.SaveChangesAsync();
        return Match;
    }
    public async Task<List<Match>> AddRange(List<Match> matches){
        _context.Set<Match>().AddRange(matches);
        _context.SaveChanges();
        return matches;
    }
    public async Task<Match> DeleteAsync(Match Match)
    {
        var removedEntity = _context.Set<Match>().Remove(Match).Entity;
        await _context.SaveChangesAsync();
        return removedEntity;
    }
    public async Task<List<Match>> GetAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;

        var query = await _context.Set<Match>()
            .OrderByDescending(n => n.Round).ThenBy(n => n.MatchOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return query;
    }
    public async Task<Match> GetByIdAsync(string id)
    {
        var entity = await _context.Matches.Where(t => t.Id.Equals(id)).FirstOrDefaultAsync();
        return entity;
    }
    public async Task<IEnumerable<Match>> GetBySpecificationAsync(MatchSpecification spec, int page, int pageSize, CancellationToken token = default)
    {
        IQueryable<Match> query = _context.Matches.OrderByDescending(n => n.StartTime);

        query = query.ApplySpecification(spec).Skip((page - 1) * pageSize).Take(pageSize);

        return await query.ToListAsync(cancellationToken: token);
    }
    public async Task<List<Match>> GetTournamentStructureAsync(string tournamentId)
    {
        var matches = await _context.Matches
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.Round)
            .ThenBy(m => m.MatchOrder)
            .ToListAsync();
        return matches;
    }
    public async Task<Match> UpdateAsync(Match Match)
    {
        _context.Entry(Match).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Match;
    }
    public async Task<Match> GetOneBySpecificationAsync(MatchSpecification spec, CancellationToken token = default)
    {
        IQueryable<Match> query = _context.Matches.OrderByDescending(n => n.StartTime);

        var res = query.ApplySpecification(spec).FirstOrDefault();

        return res;
    }
}
