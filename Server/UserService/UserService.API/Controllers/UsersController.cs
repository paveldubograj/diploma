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
        UserProfileDto userDto = await _userManageService.GetByIdAsync(User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

        return Ok(userDto);
    }

    [HttpPut]
    [Route("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] UserUpdateDto dto)
    {
        UserProfileDto userDto = await _userManageService.UpdateAsync(User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value, dto);

        return Ok(userDto);
    }

    [HttpDelete]
    [Route("profile")]
    [Authorize]
    public async Task<IActionResult> DeleteProfileAsync()
    {
        UserProfileDto userDto = await _userManageService.DeleteAsync(User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

        return Ok(userDto);
    }


    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
    {
        UserProfileDto userDto = await _userManageService.GetByIdAsync(id);
        return Ok(userDto);
    }

    [HttpGet]
    [Route("")]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<IActionResult> GetByNameAsync(int page, int pageSize, [FromQuery] string? userName, CancellationToken token = default)
    {
        return Ok(await _userManageService.GetByNameAsync(page, pageSize, userName, token));
    }

    [HttpPut]
    [Route("{id}")]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<IActionResult> EditAsync([FromRoute] string id, [FromBody] UserUpdateDto dto)
    {
        UserProfileDto userDto = await _userManageService.UpdateAsync(id, dto);

        return Ok(userDto);
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(Roles = RoleName.Admin)]
    public async Task<IActionResult> DeleteAsync([FromRoute] string id)
    {
        UserProfileDto deletedUser = await _userManageService.DeleteAsync(id);

        return Ok(deletedUser);
    }

    [HttpPost]
    [Route("profile/image")]
    [Authorize]
    public async Task<IActionResult> AddUserImageAsync(IFormFile image)
    {
        var newsDto = await _userManageService.AddImageAsync(User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value, image);

        return Ok(newsDto);
    }

    [HttpDelete]
    [Route("profile/image")]
    [Authorize]
    public async Task<IActionResult> DeleteUserImageAsync()
    {
        var newsDto = await _userManageService.RemoveImageAsync(User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

        return Ok(newsDto);
    }

    [HttpPut]
    [Route("profile/image")]
    [Authorize]
    public async Task<IActionResult> UpdateUserImageAsync(IFormFile image)
    {
        var newsDto = await _userManageService.RemoveImageAsync(User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);
        var res = await _userManageService.AddImageAsync(User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value, image);

        return Ok(newsDto);
    }
}
