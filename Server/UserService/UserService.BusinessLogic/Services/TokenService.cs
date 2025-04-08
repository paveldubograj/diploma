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

    private async Task<List<Claim>> AddClaims(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(ErrorName.UserNotFound);
        }
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, user.Id),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToUniversalTime().ToString()),
        };

        var roles = await _userRepository.GetRolesAsync(user);

        claims.AddRange(roles.Select(role =>
            new Claim(ClaimTypes.Role, role)));
        
        return claims;
    }

    public async Task<string> GenerateToken(string userId)
    {
        //var claims = await AddClaims(userId);
        var user = await _userRepository.GetByIdAsync(userId);
        Console.WriteLine(user.Email);

        var keyBytes = Encoding.UTF8.GetBytes("12345678901234567890123456789012");

        var roles = await _userRepository.GetRolesAsync(user);

        List<Claim> claims = [new Claim(ClaimTypes.Name, user.Id)];
        claims.AddRange(roles.Select(role =>
            new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(10), // Время жизни токена
            Issuer = "https://id.CompanyName.com",
            Audience = "https://tournament.CompanyName.com",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
        //var token = GenerateJwtToken(request.Username, secretKey);
        //var token1 = tokenHandler.WriteToken(token);
        
        // var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12345678901234567890123456789012"));//_options.Value.Key));
        // var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        // var token = new JwtSecurityToken(
        //     issuer: "https://id.CompanyName.com",//_options.Value.Issuer,
        //     audience: "https://tournament.CompanyName.com",//_options.Value.Audience,
        //     claims: claims,
        //     expires: DateTime.UtcNow.AddDays(3),
        //     signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
