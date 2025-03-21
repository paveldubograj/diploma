using System;
using Microsoft.EntityFrameworkCore;
using NewsService.DataAccess.Database;
using NewsService.DataAccess.Entities;
using NewsService.DataAccess.Repositories.Interfaces;

namespace NewsService.DataAccess.Repositories;

public class TagRepository : ITagRepository
{
    private NewsContext _context;

    public TagRepository(NewsContext context)
    {
        _context = context;
    }
    public async Task<Tag> AddAsync(Tag tag)
    {
        _context.Entry(tag).State = EntityState.Added;
        await _context.SaveChangesAsync();
        return tag;
    }
    public async Task<Tag> DeleteAsync(Tag tag)
    {
        var removedEntity = _context.Set<Tag>().Remove(tag).Entity;
        await _context.SaveChangesAsync();
        return removedEntity;
    }
    public async Task<Tag> GetByIdAsync(string id)
    {
        var entity = await _context.Set<Tag>().FindAsync(id);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task<Tag> UpdateAsync(Tag tag)
    {
        _context.Entry(tag).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return tag;
    }
    public async Task<List<Tag>> GetAllAsync()
    {
        return await _context.Tags.ToListAsync();
    }
}
