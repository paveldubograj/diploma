using System;
using AutoMapper;
using UserService.BusinessLogic.Models.Role;
using UserService.BusinessLogic.Models.User;
using UserService.DataAccess.Entities;

namespace UserService.BusinessLogic.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<string, RoleDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src));

        CreateMap<User, UserAuthorizedDto>().ReverseMap();
        CreateMap<User, UserCleanDto>().ReverseMap();
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash))
            .ReverseMap();
    }
}
