using System;
using DisciplineService.DataAccess.DataBase;
using DisciplineService.DataAccess.Entities;
using DisciplineService.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DisciplineService.DataAccess.Repositories;

public class DisciplineRepository : IDisciplineRepository
{
    private readonly DisciplineContext _context;
    public DisciplineRepository(DisciplineContext context){
        _context = context;
    }
    public async Task<Discipline> AddAsync(Discipline discipline)
    {
        _context.Entry(discipline).State = EntityState.Added;
        await _context.SaveChangesAsync();
        return discipline;
    }

    public async Task<Discipline> DeleteAsync(Discipline discipline)
    {
        var removedEntity = _context.Set<Discipline>().Remove(discipline).Entity;
        await _context.SaveChangesAsync();
        return removedEntity;
    }

    public async Task<List<Discipline>> GetAllAsync()
    {
        return await _context.disciplines.ToListAsync();
    }

    public async Task<Discipline> UpdateAsync(Discipline discipline)
    {
        _context.Entry(discipline).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return discipline;
    }

    public async Task<Discipline>? GetByIdAsync(string id)
    {
        return await _context.disciplines.FirstOrDefaultAsync(t => t.Id.Equals(id));
    }
}
