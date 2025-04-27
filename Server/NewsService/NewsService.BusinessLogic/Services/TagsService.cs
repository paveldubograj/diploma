using System;
using System.Threading.Tasks;
using AutoMapper;
using NewsService.BusinessLogic.Models.Tag;
using NewsService.BusinessLogic.Services.Interfaces;
using NewsService.DataAccess.Entities;
using NewsService.DataAccess.Repositories.Interfaces;
using NewsService.Shared.Constants;
using NewsService.Shared.Exeptions;

namespace NewsService.BusinessLogic.Services;

public class TagsService : ITagsService
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    public TagsService(ITagRepository tagRepository, IMapper mapper){
        _tagRepository = tagRepository;
        _mapper = mapper;
    }
    public async Task<TagDto> AddAsync(string tagDto)
    {
        TagDto dto = new TagDto(){Name = tagDto};
        var obj = _mapper.Map<Tag>(dto);
        var res = await _tagRepository.AddAsync(obj);
        return _mapper.Map<TagDto>(res);
    }

    public async Task<TagDto> DeleteAsync(string id)
    {
        var obj = await _tagRepository.GetByIdAsync(id);
        if(obj == null){
            throw new NotFoundException(ErrorName.TagNotFound);
        }
        var res = await _tagRepository.DeleteAsync(obj);
        return _mapper.Map<TagDto>(res);
    }

    public async Task<List<TagDto>> GetAllAsync()
    {
        var res = await _tagRepository.GetAllAsync();
        return _mapper.Map<List<TagDto>>(res);
    }

    public async Task<List<TagDto>> GetByStrAsync(string str)
    {
        var res = await _tagRepository.GetAllByNameAsync(str);
        return _mapper.Map<List<TagDto>>(res);
    }

    public async Task<TagDto> UpdateAsync(string id, TagDto tagDto)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if(tag == null){
            throw new NotFoundException(ErrorName.TagNotFound);
        }
        var newsUp = _mapper.Map(tagDto, tag);
        var res = _tagRepository.UpdateAsync(newsUp);
        return _mapper.Map<TagDto>(res);
    }
}
