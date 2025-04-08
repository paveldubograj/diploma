using System;
using System.Text.RegularExpressions;
using AutoMapper;
using MatchService.BusinessLogic.Models.Match;

namespace MatchService.BusinessLogic.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MatchService.DataAccess.Entities.Match, MatchCleanDto>().ReverseMap();
        CreateMap<MatchService.DataAccess.Entities.Match, MatchDto>().ReverseMap();
        CreateMap<MatchService.DataAccess.Entities.Match, MatchListDto>().ReverseMap();
    }
}
