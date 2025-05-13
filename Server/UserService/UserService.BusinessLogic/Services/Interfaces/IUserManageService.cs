using System;
using UserService.BusinessLogic.Models.User;

namespace UserService.BusinessLogic.Services.Interfaces;

public interface IUserManageService
{
    Task<UserCleanDto> GetByIdAsync(string id);
    Task<IEnumerable<UserCleanDto>> GetByNameAsync(int page, int pageSize, string firstName, CancellationToken token = default);
    Task<UserDto> UpdateAsync(string id, UserCleanDto dto);
    Task<UserCleanDto> DeleteAsync(string id);
    Task<bool> IsUserExits(string id);
    Task<int> GetTotalAsync();
}
