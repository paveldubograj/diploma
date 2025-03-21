using System;
using System.Text.RegularExpressions;
using AutoMapper;
using MatchService.BusinessLogic.Models.Match;

namespace MatchService.BusinessLogic.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Match, MatchCleanDto>().ReverseMap();
        CreateMap<Match, MatchDto>().ReverseMap();
        CreateMap<Match, MatchListDto>().ReverseMap();
    }
}
