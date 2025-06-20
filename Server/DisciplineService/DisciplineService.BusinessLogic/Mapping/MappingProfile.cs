using System;
using AutoMapper;
using DisciplineService.BusinessLogic.Models;
using DisciplineService.DataAccess.Entities;

namespace DisciplineService.BusinessLogic.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Discipline, DisciplineDto>().ReverseMap();
        CreateMap<Discipline, DisciplineCleanDto>().ReverseMap();
        CreateMap<Discipline, DisciplineCreateDto>().ReverseMap();
    }
}
