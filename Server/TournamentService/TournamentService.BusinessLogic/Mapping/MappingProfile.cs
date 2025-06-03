using System;
using AutoMapper;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Models.ParticipantDtos;
using TournamentService.BusinessLogic.Models.Tournament;
using TournamentService.DataAccess.Entities;

namespace TournamentService.BusinessLogic.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Participant, ParticipantDto>().ReverseMap();
        CreateMap<Participant, ParticipantAddDto>().ReverseMap();
        CreateMap<Participant, ParticipantSListDto>();
        CreateMap<Participant, RegisterForTournamentDto>().ReverseMap();
        CreateMap<Tournament, TournamentCleanDto>().ReverseMap();
        CreateMap<Tournament, TournamentDto>().ReverseMap();
        CreateMap<Tournament, TournamentCreateDto>().ReverseMap();
        //CreateMap<TournamentService.BusinessLogic.Protos.Match, MatchDto>().ReverseMap();
    }
}
