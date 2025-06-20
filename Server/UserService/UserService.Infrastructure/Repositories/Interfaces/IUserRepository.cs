using System;
using Microsoft.AspNetCore.Identity;
using UserService.DataAccess.Entities;
using UserService.DataAccess.Specifications;

namespace UserService.DataAccess.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<IdentityResult> AddAsync(User entity, string password);
    Task<bool> CheckPasswordAsync(User entity, string password);
    Task<IEnumerable<string>> GetRolesAsync(User user);
    Task<IdentityResult> AddToRoleAsync(User user, string role);
    Task<IdentityResult> RemoveFromRolesAsync(User user, string role);
    Task<User?> GetByIdAsync(string id);
    Task<IEnumerable<User>> GetBySpecAsync(int page, int pageSize, UserSpecification spec, CancellationToken token = default);
    Task<IdentityResult> UpdateAsync(User entity);
    Task<IdentityResult> DeleteAsync(User entity);
    Task<User> AddUserTournament(User user, string tournamentId);
    Task<User> RemoveUserTournament(User user, string tournamentId);
    Task<int> GetTotalAsync();
}
