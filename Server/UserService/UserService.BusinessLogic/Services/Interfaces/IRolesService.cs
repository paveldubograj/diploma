using System;
using UserService.BusinessLogic.Models.Role;

namespace UserService.BusinessLogic.Services.Interfaces;

public interface IRolesService
{
    Task<IEnumerable<RoleDto>> GetUserRolesAsync(string id);
    Task AddUserRoleAsync(string id, string role);
    Task RemoveUserRoleAsync(string id, string role);
}
