using System;
using AutoMapper;
using UserService.BusinessLogic.Models.Role;
using UserService.BusinessLogic.Services.Interfaces;
using UserService.DataAccess.Repositories.Interfaces;
using UserService.Shared.Contants;
using UserService.Shared.Exceptions;

namespace UserService.BusinessLogic.Services;

public class RolesService : IRolesService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public RolesService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }

        var roles = await _userRepository.GetRolesAsync(user);
        var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);

        return roleDtos;
    }

    public async Task AddUserRoleAsync(string id, string role)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }

        switch (role)
        {
            case "user":
                await _userRepository.AddToRoleAsync(user, RoleName.User);
                break;
            
            case "organizer":
                await _userRepository.AddToRoleAsync(user, RoleName.Organizer);
                break;

            case "newsteller":
                await _userRepository.AddToRoleAsync(user, RoleName.NewsTeller);
                break;
            
            case "admin":
                await _userRepository.AddToRoleAsync(user, RoleName.Admin);
                break;
            
            default:
                throw new NotFoundException(ErrorName.RoleNotFound);
        }
    }

    public async Task RemoveUserRoleAsync(string id, string role)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }

        switch (role)
        {
            case "user":
                await _userRepository.RemoveFromRolesAsync(user, RoleName.User);
                break;
            
            case "organizer":
                await _userRepository.RemoveFromRolesAsync(user, RoleName.Organizer);
                break;

            case "newsteller":
                await _userRepository.RemoveFromRolesAsync(user, RoleName.NewsTeller);
                break;
            
            case "admin":
                await _userRepository.RemoveFromRolesAsync(user, RoleName.Admin);
                break;
            
            default:
                throw new NotFoundException(ErrorName.RoleNotFound);
        }
    }
}
