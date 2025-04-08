using System;
using AutoMapper;
using NewsService.BusinessLogic.Models.News;
using NewsService.BusinessLogic.Models.Tag;
using NewsService.DataAccess.Entities;

namespace NewsService.BusinessLogic.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<News, NewsCleanDto>().ReverseMap();
        CreateMap<News, NewsUpdateDto>().ReverseMap();
        CreateMap<News, NewsDto>().ForMember(dst => dst.tags, opt => opt.MapFrom(src => src.Tags)).ReverseMap();
        CreateMap<Tag, TagDto>().ReverseMap();
    }
}
