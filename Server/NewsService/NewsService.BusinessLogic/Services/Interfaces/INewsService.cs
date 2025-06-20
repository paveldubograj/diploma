using System;
using Microsoft.AspNetCore.Http;
using NewsService.BusinessLogic.Models.Filter;
using NewsService.BusinessLogic.Models.News;

namespace NewsService.BusinessLogic.Services.Interfaces;

public interface INewsService
{
    public Task<List<NewsCleanDto>> GetAllByPageAsync(int page, int pageSize);
    public Task<NewsPagedResponse> GetByFilterAsync(NewsFilter filter);
    public Task<NewsPagedResponse> GetByUserAsync(string userId, int page, int pageSize);
    public Task<NewsDto> GetByIdAsync(string id);
    public Task<NewsCleanDto> DeleteAsync(string id);
    public Task<NewsDto> UpdateAsync(string id, NewsUpdateDto newsDto, string userId, bool isAdmin = false);
    public Task<NewsDto> UpdateVisibilityAsync(string id, bool visibility);
    public Task<NewsDto> AddTagAsync(string id, string tagId, string userId, bool isAdmin = false);
    public Task<NewsDto> RemoveTagAsync(string id, string tagId, string userId, bool isAdmin = false);
    public Task<NewsDto> AddAsync(NewsUpdateDto newsDto, string userId, string userName);
    public Task<NewsDto> AddImageAsync(string id, IFormFile file, string userId);
    public Task<NewsDto> DeleteImageAsync(string id, string userId);
}
