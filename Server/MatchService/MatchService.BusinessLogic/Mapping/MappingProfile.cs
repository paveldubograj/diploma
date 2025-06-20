using System;
using System.Text.RegularExpressions;
using AutoMapper;
using MatchService.BusinessLogic.Models.Match;
using MatchService.DataAccess.Entities;

namespace MatchService.BusinessLogic.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MatchService.DataAccess.Entities.Match, MatchDto>().ReverseMap();
        CreateMap<MatchService.DataAccess.Entities.Match, MatchListDto>().ReverseMap();
        CreateMap<MatchService.DataAccess.Entities.Match, MatchUpdateDto>().ReverseMap();
        CreateMap<MatchList, MatchPagedResponse>();
    }
}
