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
        UserCleanDto userDto = await _userManageService.GetByIdAsync(User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

        return Ok(userDto);
    }

    [HttpPut]
    [Route("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] UserCleanDto dto)
    {
        UserDto userDto = await _userManageService.UpdateAsync(User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value, dto);

        return Ok(userDto);
    }

    [HttpDelete]
    [Route("profile")]
    [Authorize]
    public async Task<IActionResult> DeleteProfileAsync()
    {
        UserCleanDto userDto = await _userManageService.DeleteAsync(User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

        return Ok(userDto);
    }


    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetAsync([FromRoute] string id)
    {
        UserCleanDto userDto = await _userManageService.GetByIdAsync(id);
        
        return Ok(userDto);
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetByNameAsync(int page, int pageSize, [FromQuery] string? userName, CancellationToken token = default)
    {
        IEnumerable<UserCleanDto> result = await _userManageService.GetByNameAsync(page, pageSize, userName, token);
        
        return Ok(result);
    }

    [Authorize(Roles = RoleName.Admin)]
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> EditAsync([FromRoute] string id, [FromBody] UserCleanDto dto)
    {
        UserDto userDto = await _userManageService.UpdateAsync(id, dto);
        
        return Ok(userDto);
    }
    
    [Authorize(Roles = RoleName.Admin)]
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute]string id)
    {
        UserCleanDto deletedUser = await _userManageService.DeleteAsync(id);
        
        return Ok(deletedUser);
    }
}
