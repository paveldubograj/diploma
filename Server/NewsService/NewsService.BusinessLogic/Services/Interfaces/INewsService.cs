using System;
using NewsService.BusinessLogic.Models.Filter;
using NewsService.BusinessLogic.Models.News;

namespace NewsService.BusinessLogic.Services.Interfaces;

public interface INewsService
{
    public Task<List<NewsCleanDto>> GetAllByPageAsync(int page, int pageSize);
    public Task<List<NewsCleanDto>> GetByFilterAsync(NewsFilter filter, int page, int pageSize);
    public Task<NewsDto> GetByIdAsync(string id);
    public Task<NewsCleanDto> DeleteAsync(string id);
    public Task<NewsDto> UpdateAsync(string id, NewsUpdateDto newsDto, string userId);
    public Task<NewsDto> AddTagAsync(string id, string tagId, string userId);
    public Task<NewsDto> AddAsync(NewsDto newsDto);
}
