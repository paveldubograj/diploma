using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TournamentService.DataAccess.Database;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.DataAccess.Specifications;
using TournamentService.DataAccess.Specifications.SpecSettings;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
using TournamentService.Shared.Exceptions;

namespace TournamentService.DataAccess.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly TournamentContext _db;
    public TournamentRepository(TournamentContext context)
    {
        _db = context;
    }
    public async Task<Tournament> AddAsync(Tournament tournament)
    {
        _db.Set<Tournament>().Add(tournament);
        await _db.SaveChangesAsync();
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
        return r;
    }

    public Tournament GetById(string id)
    {
        var t = _db.Tournaments.Find(id);
        if (t is null) throw new NotFoundException(ErrorName.TournamentNotFound);
        return t;
    }

    public async Task<TournamentList> GetBySpecificationAsync(TournamentSpecification spec1, TournamentSortOptions? options, int page, int pageSize, CancellationToken token = default)
    {
        int total = await _db.Tournaments.ApplySpecification(spec1).CountAsync();
        IQueryable<Tournament> query = _db.Tournaments
            .AsNoTracking()
            .ApplySpecification(spec1);

        switch (options)
        {
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

        query = query.Skip((page - 1) * pageSize).Take(pageSize);

        return new TournamentList(){Tournaments = await query.ToListAsync(), Total = total};
    }

    public async Task<Tournament> UpdateAsync(Tournament tournament)
    {
        _db.Entry(tournament).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return tournament;
    }

    public async Task<int> GetTotalAsync()
    {
        return await _db.Participants.CountAsync();
    }

    public async Task<bool> IsRegistrationAllowed(string tournamentId)
    {
        var t = await _db.Tournaments.FindAsync(tournamentId);
        if (t is null) throw new NotFoundException(ErrorName.TournamentNotFound);
        return t.IsRegistrationAllowed;
    }

    public async Task<List<Tournament>> GetByIdsAsync(List<string> ids)
    {
        var results = await _db.Tournaments
            .AsNoTracking()
            .Where(e => ids.Contains(e.Id))
            .ToListAsync();
        return results;
    }
}
