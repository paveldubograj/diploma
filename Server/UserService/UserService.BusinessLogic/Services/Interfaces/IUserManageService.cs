using System;
using Microsoft.AspNetCore.Http;
using UserService.BusinessLogic.Models.User;

namespace UserService.BusinessLogic.Services.Interfaces;

public interface IUserManageService
{
    Task<UserProfileDto> GetByIdAsync(string id);
    Task<IEnumerable<UserCleanDto>> GetByNameAsync(int page, int pageSize, string? firstName, CancellationToken token = default);
    Task<UserProfileDto> UpdateAsync(string id, UserProfileDto dto);
    Task<UserCleanDto> DeleteAsync(string id);
    Task<bool> IsUserExits(string id);
    Task<int> GetTotalAsync();
    Task<UserProfileDto> AddImageAsync(string id, IFormFile file);
    Task<UserProfileDto> RemoveImageAsync(string id);
    Task<UserProfileDto> RegisterForTournamentAsync(string userId, string tournamentId);
    Task<UserProfileDto> RemoveUserTournamentAsync(string userId, string tournamentId);
}
