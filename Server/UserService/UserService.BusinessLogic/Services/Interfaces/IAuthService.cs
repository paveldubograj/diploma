using System;
using UserService.BusinessLogic.Models.User;

namespace UserService.BusinessLogic.Services.Interfaces;

public interface IAuthService
{
    Task<UserAuthorizedDto> LogInAsync(LogInDto dto);
    Task RegistrationAsync(UserDto dto);
}
