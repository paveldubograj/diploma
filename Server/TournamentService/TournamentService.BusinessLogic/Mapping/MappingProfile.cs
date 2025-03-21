using System;
using AutoMapper;
using TournamentService.BusinessLogic.Models.ParticipantDtos;
using TournamentService.BusinessLogic.Models.Tournament;
using TournamentService.DataAccess.Entities;

namespace TournamentService.BusinessLogic.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Participant, ParticipantDto>().ReverseMap();
        CreateMap<Tournament, TournamentCleanDto>().ReverseMap();
        CreateMap<Tournament, TournamentDto>().ReverseMap();
    }
}
