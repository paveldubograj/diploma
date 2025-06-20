using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NewsService.BusinessLogic.Models.Filter;
using NewsService.BusinessLogic.Models.News;
using NewsService.BusinessLogic.Services.Interfaces;
using NewsService.DataAccess.Entities;
using NewsService.DataAccess.Repositories.Interfaces;
using NewsService.DataAccess.Specifications;
using NewsService.Shared.Constants;
using NewsService.Shared.Exeptions;
using Newtonsoft.Json;

namespace NewsService.BusinessLogic.Services;

public class NewsService : INewsService
{
    private readonly ICacheService _cacheService;
    private readonly INewsRepository _newsRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IImageService _imageService;
    private readonly IDisciplineGrpcService _disciplineService;
    private readonly IMapper _mapper;
    public NewsService(INewsRepository newsRepository, ITagRepository tagRepository, IMapper mapper, IImageService imageService, IDisciplineGrpcService disciplineService, ICacheService cacheService)
    {
        _cacheService = cacheService;
        _newsRepository = newsRepository;
        _tagRepository = tagRepository;
        _imageService = imageService;
        _disciplineService = disciplineService;
        _mapper = mapper;
    }
    public async Task<NewsDto> AddAsync(NewsUpdateDto newsDto, string userId, string userName)
    {
        var news = _mapper.Map<News>(newsDto);
        if (!await _disciplineService.IsDisciplineExists(news.CategoryId)) throw new NotFoundException(ErrorName.DisciplineNotFound);
        news.AuthorId = userId;
        news.AuthorName = userName;
        news.PublishingDate = DateTime.Now;
        var result = await _newsRepository.AddAsync(news);
        return _mapper.Map<NewsDto>(result);
    }

    public async Task<NewsCleanDto> DeleteAsync(string id)
    {
        var obj = await _newsRepository.GetByIdAsync(id);
        if (obj == null)
        {
            throw new NotFoundException(ErrorName.NewsNotFound);
        }
        var result = await _newsRepository.DeleteAsync(obj);
        return _mapper.Map<NewsCleanDto>(result);
    }

    public async Task<List<NewsCleanDto>> GetAllByPageAsync(int page, int pageSize)
    {
        var list = await _newsRepository.GetAsync(page, pageSize);
        return _mapper.Map<List<NewsCleanDto>>(list);
    }

    public async Task<NewsPagedResponse> GetByFilterAsync(NewsFilter filter)
    {
        var serializedValue = JsonConvert.SerializeObject(filter);
        var cache = await _cacheService.GetAsync<NewsPagedResponse>(serializedValue);
        if (cache != null) return cache;
        NewsSpecification specification = NewsSpecification.FilterNews(filter.SearchString, filter.CategoryId, filter.Tags);
        var list = await _newsRepository.GetBySpecificationAsync(specification, filter.Page, filter.PageSize, filter.sortOptions);
        var result = _mapper.Map<NewsPagedResponse>(list);
        await _cacheService.SetAsync<NewsPagedResponse>(serializedValue, result);
        return result;
    }

    public async Task<NewsPagedResponse> GetByUserAsync(string userId, int page, int pageSize)
    {
        NewsSpecification specification = new NewsSpecification(c => c.AuthorId.Equals(userId));
        var result = await _newsRepository.GetBySpecificationAsync(specification, page, pageSize, null);
        return _mapper.Map<NewsPagedResponse>(result);
    }

    public async Task<NewsDto> GetByIdAsync(string id)
    {
        var res = await _newsRepository.GetByIdAsync(id);
        if (res == null)
        {
            throw new NotFoundException(ErrorName.NewsNotFound);
        }
        return _mapper.Map<NewsDto>(res);
    }

    public async Task<NewsDto> UpdateVisibilityAsync(string id, bool visibility)
    {
        var news = await _newsRepository.GetByIdAsync(id);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.NewsNotFound);
        }
        news.Visibility = visibility;
        var res = await _newsRepository.UpdateAsync(news);
        return _mapper.Map<NewsDto>(res);
    }

    public async Task<NewsDto> UpdateAsync(string id, NewsUpdateDto newsDto, string userId, bool isAdmin = false)
    {
        var news = await _newsRepository.GetByIdAsync(id);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.NewsNotFound);
        }
        if (!news.AuthorId.Equals(userId) && !isAdmin)
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var newsUp = _mapper.Map(newsDto, news);
        if (!await _disciplineService.IsDisciplineExists(newsUp.CategoryId)) throw new NotFoundException(ErrorName.DisciplineNotFound);
        var res = await _newsRepository.UpdateAsync(newsUp);
        return _mapper.Map<NewsDto>(res);
    }

    public async Task<NewsDto> AddTagAsync(string id, string tagId, string userId, bool isAdmin = false)
    {
        var news = await _newsRepository.GetByIdAsync(id);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.NewsNotFound);
        }
        if (!news.AuthorId.Equals(userId) && !isAdmin)
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var tag = await _tagRepository.GetByIdAsync(tagId);
        if (tag == null)
        {
            throw new NotFoundException(ErrorName.TagNotFound);
        }
        news.Tags.Add(tag);
        var res = await _newsRepository.UpdateAsync(news);
        return _mapper.Map<NewsDto>(res);
    }

    public async Task<NewsDto> RemoveTagAsync(string id, string tagId, string userId, bool isAdmin = false)
    {
        var news = await _newsRepository.GetByIdAsync(id);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.NewsNotFound);
        }
        if (!news.AuthorId.Equals(userId) && !isAdmin)
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        var tag = await _tagRepository.GetByIdAsync(tagId);
        if (tag == null)
        {
            throw new NotFoundException(ErrorName.TagNotFound);
        }
        news.Tags.Remove(tag);
        var res = await _newsRepository.UpdateAsync(news);
        return _mapper.Map<NewsDto>(res);
    }

    public async Task<NewsDto> AddImageAsync(string id, IFormFile file, string userId)
    {
        var news = await _newsRepository.GetByIdAsync(id);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.NewsNotFound);
        }
        if (!news.AuthorId.Equals(userId))
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        news.ImagePath = await _imageService.SaveImage(file, id);
        Console.WriteLine("ImagePath from news service: " + news.ImagePath);
        var res = await _newsRepository.UpdateAsync(news);
        Console.WriteLine("ImagePath from news service after update: " + news.ImagePath);
        return _mapper.Map<NewsDto>(res);
    }

    public async Task<NewsDto> DeleteImageAsync(string id, string userId)
    {
        var news = await _newsRepository.GetByIdAsync(id);
        if (news == null)
        {
            throw new NotFoundException(ErrorName.NewsNotFound);
        }
        if (!news.AuthorId.Equals(userId))
        {
            throw new BadAuthorizeException(ErrorName.YouAreNotAllowed);
        }
        _imageService.DeleteImage(news.ImagePath);
        news.ImagePath = string.Empty;
        var res = await _newsRepository.UpdateAsync(news);
        return _mapper.Map<NewsDto>(res);
    } 
}
