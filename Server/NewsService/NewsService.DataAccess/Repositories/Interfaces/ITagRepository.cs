using System;
using NewsService.DataAccess.Entities;

namespace NewsService.DataAccess.Repositories.Interfaces;

public interface ITagRepository
{
    Task<Tag> GetByIdAsync(string id);
    Task<Tag> AddAsync(Tag tag);
    Task<Tag> DeleteAsync(Tag tag);
    Task<Tag> UpdateAsync(Tag tag);
    Task<List<Tag>> GetAllAsync();
}
