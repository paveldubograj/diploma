using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.BusinessLogic.Services.Interfaces;
using UserService.Shared.Contants;

namespace UserService.API.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpGet]
        [Route("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserRolesAsync([FromRoute] string userId)
        {
            var result = await _rolesService.GetUserRolesAsync(userId);
            
            return Ok(result);
        }
        
        [HttpPost]
        [Route("{userId}/{role}")]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<IActionResult> AddRoleAsync([FromRoute] string userId, [FromRoute] string role)
        {
            await _rolesService.AddUserRoleAsync(userId, role);
            
            return Ok();
        }
        
        [HttpDelete]
        [Route("{userId}/{role}")]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<IActionResult> RemoveRoleAsync([FromRoute] string userId, [FromRoute] string role)
        {
            await _rolesService.RemoveUserRoleAsync(userId, role);
            
            return Ok();
        }
    }
}
