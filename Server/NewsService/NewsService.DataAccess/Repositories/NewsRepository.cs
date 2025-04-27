using System;
using Microsoft.EntityFrameworkCore;
using NewsService.DataAccess.Database;
using NewsService.DataAccess.Entities;
using NewsService.DataAccess.Repositories.Interfaces;
using NewsService.DataAccess.Specifications;
using NewsService.DataAccess.Specifications.SpecSettings;
using NewsService.Shared.Enums;

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
    public async Task<int> GetTotalAsync(){
        return await db.News.CountAsync();
    }
    public async Task<IEnumerable<News>> GetBySpecificationAsync(NewsSpecification spec, List<Tag> tags, int page, int pageSize, SortOptions? options, CancellationToken token = default)
    {
        IQueryable<News> query = db.News.OrderByDescending(n => n.PublishingDate).Include(n => n.Tags);

        query = query.ApplySpecification(spec);
        IQueryable<News> r = Enumerable.Empty<News>().AsQueryable();

        if(tags.Count > 0){
            bool i = true;
            foreach(var elem in query)
            { 
                foreach(var el in tags) 
                {
                    if(!elem.Tags.Contains(el)) 
                    {
                        i = false;
                        break;
                    }
                }
                if(i)r.Append(elem);
                i = true;
            }
        }
        else r = query;

        r = r.Skip((page - 1) * pageSize).Take(pageSize);

        switch(options){
            case SortOptions.ByName:
                r = r.OrderBy(c => c.Title);
                break;
            case SortOptions.ByNameDesc:
                r = r.OrderByDescending(c => c.Title);
                break;
            case SortOptions.ByDateDesc:
                r = r.OrderByDescending(c => c.PublishingDate);
                break;
            default:
                r = r.OrderBy(c => c.PublishingDate);
                break;
        }

        //return await query.ToListAsync(cancellationToken: token);
        return r;
    }
    public async Task<News> GetByIdAsync(string id)
    {
        var entity = await db.News.Include(t => t.Tags).Where(t => t.Id.Equals(id)).FirstOrDefaultAsync();
        return entity;
    }
    public async Task<News> UpdateAsync(News news)
    {
        db.Entry(news).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return news;
    }
}
