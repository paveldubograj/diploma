using System;
using AutoMapper;
using UserService.BusinessLogic.Models.User;
using UserService.BusinessLogic.Services.Interfaces;
using UserService.DataAccess.Entities;
using UserService.DataAccess.Repositories.Interfaces;
using UserService.Shared.Contants;
using UserService.Shared.Exceptions;

namespace UserService.BusinessLogic.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    
    public AuthService(IUserRepository userRepository, IMapper mapper, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _tokenService = tokenService;
    }
    
    public async Task<UserAuthorizedDto> LogInAsync(LogInDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        
        if (user == null)
        {
            throw new NotFoundException(ErrorName.EmailNotFound);
        }

        if (!await _userRepository.CheckPasswordAsync(user, dto.Password))
        {
            throw new BadAuthorizeException(ErrorName.PasswordInvalid);
        }

        var accessToken = await _tokenService.GenerateToken(user.Id);
        var userDto = _mapper.Map<UserAuthorizedDto>(user);
        userDto.AccessToken = accessToken;
        
        return userDto;
    }

    public async Task RegistrationAsync(UserDto dto)
    {
        if (await IsEmailExist(dto.Email))
        {
            throw new BadAuthorizeException(ErrorName.UserAlreadyExist);
        }
        
        var user = _mapper.Map<User>(dto);
        var result = await _userRepository.AddAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join(
                Environment.NewLine,
                result.Errors.Select(exception =>
                    exception.Description
                ));
            
            throw new IdentityException(errorMessage);
        }
    }
    
    private async Task<bool> IsEmailExist(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        
        return user != null;
    }
}
