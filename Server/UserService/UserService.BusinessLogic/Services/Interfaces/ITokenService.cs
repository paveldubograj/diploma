using System;

namespace UserService.BusinessLogic.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateToken(string userId);
}
