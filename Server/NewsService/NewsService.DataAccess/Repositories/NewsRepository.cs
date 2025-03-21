using System;
using Microsoft.EntityFrameworkCore;
using NewsService.DataAccess.Database;
using NewsService.DataAccess.Entities;
using NewsService.DataAccess.Repositories.Interfaces;
using NewsService.DataAccess.Specifications;
using NewsService.DataAccess.Specifications.SpecSettings;

namespace NewsService.DataAccess.Repositories;

public class NewsRepository : INewsRepository
{

    private NewsContext db;

    public NewsRepository(NewsContext db)
    {
        this.db = db;
    }
    public async Task<News> AddAsync(News news)
    {
        db.Entry(news).State = EntityState.Added;
        await db.SaveChangesAsync();
        return news;
    }
    public async Task<News> DeleteAsync(News news)
    {
        var removedEntity = db.Set<News>().Remove(news).Entity;
        await db.SaveChangesAsync();
        return removedEntity;
    }
    public async Task<List<News>> GetAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;

        var query = await db.News
            .OrderByDescending(n => n.PublishingDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return query;
    }
    public async Task<IEnumerable<News>> GetBySpecificationAsync(NewsSpecification spec, int page, int pageSize, CancellationToken token = default)
    {
        IQueryable<News> query = db.News.OrderByDescending(n => n.PublishingDate).Include(n => n.Tags);

        query = query.ApplySpecification(spec).Skip((page - 1) * pageSize).Take(pageSize);

        return await query.ToListAsync(cancellationToken: token);
    }
    public async Task<News> GetByIdAsync(string id)
    {
        var entity = await db.News.Include(t => t.Tags).Where(t => t.Id.Equals(id)).FirstOrDefaultAsync();
        return entity;
    }
    public async Task<News> UpdateAsync(News news)
    {
        db.Entry(news).State = EntityState.Modified;
        return news;
    }
}
