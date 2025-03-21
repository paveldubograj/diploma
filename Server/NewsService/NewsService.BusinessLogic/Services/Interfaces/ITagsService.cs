using System;
using NewsService.BusinessLogic.Models.Tag;

namespace NewsService.BusinessLogic.Services.Interfaces;

public interface ITagsService
{
    public Task<List<TagDto>> GetAllAsync();
    public Task<TagDto> DeleteAsync(string id);
    public Task<TagDto> UpdateAsync(string id, TagDto tagDto);
    public Task<TagDto> AddAsync(TagDto tagDto);
}
