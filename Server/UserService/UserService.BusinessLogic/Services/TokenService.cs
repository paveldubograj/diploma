using System;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using UserService.BusinessLogic.Services.Interfaces;
using UserService.DataAccess.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UserService.Shared.Options;
using UserService.Shared.Exceptions;
using UserService.Shared.Contants;

namespace UserService.BusinessLogic.Services;

public class TokenService : ITokenService
{
    private readonly IUserRepository _userRepository;
    private readonly IOptions<JwtOptions> _options;

    public TokenService(IUserRepository userRepository, IOptions<JwtOptions> options)
    {
        _userRepository = userRepository;
        _options = options;
    }

    public async Task<string> GenerateToken(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        var keyBytes = Encoding.UTF8.GetBytes(_options.Value.Key);

        var roles = await _userRepository.GetRolesAsync(user);

        List<Claim> claims = [new Claim(ClaimTypes.Name, user.Id)];
        claims.Add(new Claim(ClaimTypes.GivenName, user.UserName));
        claims.AddRange(roles.Select(role =>
            new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_options.Value.Expires), // Время жизни токена
            Issuer = _options.Value.Issuer,
            Audience = _options.Value.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
