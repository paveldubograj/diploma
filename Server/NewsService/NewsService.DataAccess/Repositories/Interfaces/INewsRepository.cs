using System;
using NewsService.DataAccess.Entities;
using NewsService.DataAccess.Specifications;
using NewsService.Shared.Enums;

namespace NewsService.DataAccess.Repositories.Interfaces;

public interface INewsRepository
{
    Task<News?> GetByIdAsync(string id);
    Task<List<News>> GetAsync(int page, int pageSize);
    Task<News> AddAsync(News news);
    Task<News> DeleteAsync(News news);
    Task<News> UpdateAsync(News news);
    Task<int> GetTotalAsync();
    Task<List<News>> GetBySpecWithNoSortAsync(NewsSpecification spec, int page, int pageSize);
    Task<IEnumerable<News>> GetBySpecificationAsync(NewsSpecification spec1, List<Tag> tags, int page, int pageSize, SortOptions? options, CancellationToken token = default);
}
