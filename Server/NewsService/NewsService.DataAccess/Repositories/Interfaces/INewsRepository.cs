using System;
using NewsService.DataAccess.Entities;
using NewsService.DataAccess.Specifications;

namespace NewsService.DataAccess.Repositories.Interfaces;

public interface INewsRepository
{
    Task<News> GetByIdAsync(string id);
    Task<List<News>> GetAsync(int page, int pageSize);
    Task<News> AddAsync(News news);
    Task<News> DeleteAsync(News news);
    Task<News> UpdateAsync(News news);
    Task<IEnumerable<News>> GetBySpecificationAsync(NewsSpecification spec1, int page, int pageSize, CancellationToken token = default);
}
