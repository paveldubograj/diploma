using System;
using AutoMapper;
using NewsService.BusinessLogic.Models.Filter;
using NewsService.BusinessLogic.Models.News;
using NewsService.BusinessLogic.Services.Interfaces;
using NewsService.DataAccess.Entities;
using NewsService.DataAccess.Repositories.Interfaces;
using NewsService.DataAccess.Specifications;
using NewsService.Shared.Constants;
using NewsService.Shared.Exeptions;

namespace NewsService.BusinessLogic.Services;

public class NewsService : INewsService
{
    private readonly INewsRepository _newsRepository;
    private readonly IMapper _mapper;
    public NewsService(INewsRepository newsRepository, IMapper mapper){
        _newsRepository = newsRepository;
        _mapper = mapper;
    }
    public async Task<NewsDto> AddAsync(NewsDto newsDto)
    {
        var news = _mapper.Map<News>(newsDto);
        var result = await _newsRepository.AddAsync(news);
        return _mapper.Map<NewsDto>(result);
    }

    public async Task<NewsCleanDto> DeleteAsync(string id)
    {
        var obj = await _newsRepository.GetByIdAsync(id);
        if(obj == null){
            throw new NotFoundException(ErrorName.NewsNotFound);
        }
        var result = _newsRepository.DeleteAsync(obj);
        return _mapper.Map<NewsCleanDto>(result);
    }

    public async Task<List<NewsCleanDto>> GetAllByPageAsync(int page, int pageSize)
    {
        var list = await _newsRepository.GetAsync(page, pageSize);
        return _mapper.Map<List<NewsCleanDto>>(list);
    }

    public async Task<List<NewsCleanDto>> GetByFilterAsync(NewsFilter filter, int page, int pageSize)
    {
        var tags = _mapper.Map<List<Tag>>(filter.Tags);
        NewsSpecification specification = NewsSpecification.FilterNews(filter.SearchString, filter.CategoryId, tags);
        var result = await _newsRepository.GetBySpecificationAsync(specification, page, pageSize);
        return _mapper.Map<List<NewsCleanDto>>(result);
    }

    public async Task<NewsDto> GetByIdAsync(string id)
    {
        var res = await _newsRepository.GetByIdAsync(id);
        if(res == null){
            throw new NotFoundException(ErrorName.NewsNotFound);
        }
        return _mapper.Map<NewsDto>(res);
    }

    public async Task<NewsDto> UpdateAsync(string id, NewsDto newsDto, string userId)
    {
        var news = await _newsRepository.GetByIdAsync(id);
        if(news == null){
            throw new NotFoundException(ErrorName.NewsNotFound);
        }
        if(!news.AuthorId.Equals(userId)){
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var newsUp = _mapper.Map(newsDto, news);
        var res = _newsRepository.UpdateAsync(newsUp);
        return _mapper.Map<NewsDto>(res);
    }
}
