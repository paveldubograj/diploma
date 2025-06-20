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
    private NewsContext _context;
    public NewsRepository(NewsContext context)
    {
        _context = context;
    }
    public async Task<News> AddAsync(News news)
    {
        _context.Entry(news).State = EntityState.Added;
        await _context.SaveChangesAsync();
        return news;
    }
    public async Task<News> DeleteAsync(News news)
    {
        var removedEntity = _context.Set<News>().Remove(news).Entity;
        await _context.SaveChangesAsync();
        return removedEntity;
    }
    public async Task<List<News>> GetAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;

        var query = await _context.News
            .Where(c => c.Visibility)
            .AsNoTracking()
            .OrderByDescending(n => n.PublishingDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return query;
    }
    public async Task<List<News>> GetBySpecWithNoSortAsync(NewsSpecification spec, int page, int pageSize)
    {
        if (page < 1) page = 1;

        var query = _context.News
            .Where(c => c.Visibility);
            
        var r = await query.ApplySpecification(spec)
            .OrderByDescending(n => n.PublishingDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return r;
    }
    public async Task<int> GetTotalAsync()
    {
        return await _context.News.CountAsync();
    }
    public async Task<IEnumerable<News>> GetBySpecificationAsync(NewsSpecification spec, List<Tag> tags, int page, int pageSize, SortOptions? options, CancellationToken token = default)
    {
        IQueryable<News> query = _context.News
            .Where(c => c.Visibility)
            .AsNoTracking()
            .Include(n => n.Tags);
            
        switch (options)
        {
            case SortOptions.ByName:
                query = query.OrderBy(c => c.Title);
                break;
            case SortOptions.ByNameDesc:
                query = query.OrderByDescending(c => c.Title);
                break;
            case SortOptions.ByDateDesc:
                query = query.OrderByDescending(c => c.PublishingDate);
                break;
            default:
                query = query.OrderBy(c => c.PublishingDate);
                break;
        }

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
                if(i) r.Append(elem);
                i = true;
            }
        }
        else r = query;

        r = r.Skip((page - 1) * pageSize).Take(pageSize);

        //return await query.ToListAsync(cancellationToken: token);
        return r;
    }
    public async Task<News?> GetByIdAsync(string id)
    {
        var entity = await _context.News.Include(t => t.Tags).Where(t => t.Id.Equals(id)).FirstOrDefaultAsync();
        return entity;
    }
    public async Task<News> UpdateAsync(News news)
    {
        _context.Entry(news).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return news;
    }
}
