using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.BusinessLogic.Models.User;
using UserService.BusinessLogic.Services.Interfaces;
using UserService.Shared.Contants;

namespace UserService.API.Controllers;

[Route("api/user")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserManageService _userManageService;

    public UsersController(IUserManageService userManageService)
    {
        _userManageService = userManageService;
    }

    [HttpGet]
    [Route("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfileAsync()
    {
        var userDto = await _userManageService.GetByIdAsync(User.Claims.First(x => x.Type.Equals(JwtRegisteredClaimNames.Jti)).Value);

        return Ok(userDto);
    }

    [HttpPut]
    [Route("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] UserDto dto)
    {
        var userDto = await _userManageService.UpdateAsync(User.Claims.First(x => x.Type.Equals(JwtRegisteredClaimNames.Jti)).Value, dto);

        return Ok(userDto);
    }

    [HttpDelete]
    [Route("profile")]
    [Authorize]
    public async Task<IActionResult> DeleteProfileAsync()
    {
        var userDto = await _userManageService.DeleteAsync(User.Claims.First(x => x.Type.Equals(JwtRegisteredClaimNames.Jti)).Value);

        return Ok(userDto);
    }


    [HttpGet]
    [Route("{id}")]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<IActionResult> GetAsync([FromRoute] string id)
    {
        var userDto = await _userManageService.GetByIdAsync(id);
        
        return Ok(userDto);
    }

    [HttpGet]
    [Route("")]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<IActionResult> GetByNameAsync([FromQuery] string userName, CancellationToken token = default)
    {
        var result = await _userManageService.GetByNameAsync(userName, token);
        
        return Ok(result);
    }

    [Authorize(Roles = RoleName.Admin)]
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> EditAsync([FromRoute] string id, [FromBody] UserDto dto)
    {
        var userDto = await _userManageService.UpdateAsync(id, dto);
        
        return Ok(userDto);
    }
    
    [Authorize(Roles = RoleName.Admin)]
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute]string id)
    {
        var deletedUser = await _userManageService.DeleteAsync(id);
        
        return Ok(deletedUser);
    }
}
