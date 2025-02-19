using System;
using AutoMapper;
using UserService.BusinessLogic.Models.User;
using UserService.BusinessLogic.Services.Interfaces;
using UserService.DataAccess.Repositories.Interfaces;
using UserService.DataAccess.Specifications;
using UserService.Shared.Contants;
using UserService.Shared.Exceptions;

namespace UserService.BusinessLogic.Services;

public class UserManageService : IUserManageService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserManageService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> GetByIdAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }
        
        var userDto = _mapper.Map<UserDto>(user);
        
        return userDto;
    }

    public async Task<IEnumerable<UserCleanDto>> GetByNameAsync(string firstName, CancellationToken token = default)
    {
        var spec = new UserSpecification(user => user.UserName == firstName);
        var users = await _userRepository.GetBySpecAsync(spec, token);
        var userDtos = _mapper.Map<IEnumerable<UserCleanDto>>(users);
        
        return userDtos;
    }


    public async Task<UserDto> UpdateAsync(string id, UserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }
        
        var userUpdated = _mapper.Map(dto, user);
        var result = await _userRepository.UpdateAsync(userUpdated);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join(
                Environment.NewLine,
                result.Errors.Select(exception =>
                    exception.Description
                ));
            
            throw new IdentityException(errorMessage);
        }
        
        return _mapper.Map<UserDto>(userUpdated);
    }

    public async Task<UserCleanDto> DeleteAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }
        
        var result = await _userRepository.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join(
                Environment.NewLine,
                result.Errors.Select(exception =>
                    exception.Description
                ));
            
            throw new IdentityException(errorMessage);
        }

        return _mapper.Map<UserCleanDto>(user);
    }
    

    public async Task<bool> IsUserExits(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        return user != null;
    }
}
