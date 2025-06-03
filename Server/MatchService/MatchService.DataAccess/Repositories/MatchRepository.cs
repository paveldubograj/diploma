using System;
using MatchService.DataAccess.Database;
using MatchService.DataAccess.Entities;
using MatchService.DataAccess.Repositories.Interfaces;
using MatchService.DataAccess.Specifications;
using MatchService.DataAccess.Specifications.SpecSettings;
using MatchService.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace MatchService.DataAccess.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly MatchContext _context;

    public MatchRepository(MatchContext context)
    {
        _context = context;
    }
    public async Task<Match> AddAsync(Match match)
    {
        _context.Entry(match).State = EntityState.Added;
        await _context.SaveChangesAsync();
        return match;
    }
    public async Task<List<Match>> AddRange(List<Match> matches){
        _context.Set<Match>().AddRange(matches);
        await _context.SaveChangesAsync();
        return matches;
    }
    public async Task<Match> DeleteAsync(Match match)
    {
        var removedEntity = _context.Set<Match>().Remove(match).Entity;
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
    public async Task<IEnumerable<Match>> GetBySpecificationAsync(MatchSpecification spec, SortOptions? options, int page, int pageSize, CancellationToken token = default)
    {
        IQueryable<Match> query = _context.Matches
            .OrderByDescending(n => n.StartTime)
            .ApplySpecification(spec)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        switch (options){
            case SortOptions.ByRound:
                query = query.OrderBy(c => c.Round);
                break;
            case SortOptions.ByRoundDesc:
                query = query.OrderByDescending(c => c.Round);
                break;
            case SortOptions.ByOrder:
                query = query.OrderBy(c => c.MatchOrder);
                break;
            case SortOptions.ByOrderDesc:
                query = query.OrderByDescending(c => c.MatchOrder);
                break;
            case SortOptions.ByDate:
                query = query.OrderBy(c => c.StartTime);
                break;
            case SortOptions.ByDateDesc:
                query = query.OrderByDescending(c => c.StartTime);
                break;
            default:
                query = query.OrderBy(c => c.Round);
                break;
        }

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
    public async Task<Match> UpdateAsync(Match match)
    {
        _context.Entry(match).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return match;
    }
    public async Task<Match> GetOneBySpecificationAsync(MatchSpecification spec, CancellationToken token = default)
    {
        IQueryable<Match> query = _context.Matches.OrderByDescending(n => n.StartTime);

        var res = await query.ApplySpecification(spec).FirstOrDefaultAsync();

        return res;
    }
    public async Task<int> GetTotalAsync(){
        return await _context.Matches.CountAsync();
    }
}
