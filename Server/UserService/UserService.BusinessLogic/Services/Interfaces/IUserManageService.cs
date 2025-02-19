using System;
using UserService.BusinessLogic.Models.User;

namespace UserService.BusinessLogic.Services.Interfaces;

public interface IUserManageService
{
    Task<UserDto> GetByIdAsync(string id);
    Task<IEnumerable<UserCleanDto>> GetByNameAsync(string firstName, CancellationToken token = default);
    Task<UserDto> UpdateAsync(string id, UserDto dto);
    Task<UserCleanDto> DeleteAsync(string id);
    Task<bool> IsUserExits(string id);
}
