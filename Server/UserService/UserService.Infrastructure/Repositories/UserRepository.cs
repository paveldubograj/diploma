using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.DataAccess.DataBase;
using UserService.DataAccess.Entities;
using UserService.DataAccess.Repositories.Interfaces;
using UserService.DataAccess.Specifications;
using UserService.DataAccess.Specifications.SpecSettings;
using UserService.Shared.Contants;
using UserService.Shared.Exceptions;

namespace UserService.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private UsersContext _db;
    private UserManager<User> _userManager;

    public UserRepository(UsersContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _db.Set<User>().Include(c => c.userTournaments).FirstOrDefaultAsync(c => c.Id.Equals(id));
    }

    public async Task<IEnumerable<User>> GetBySpecAsync(int page, int pageSize, UserSpecification spec, CancellationToken token = default)
    {
        IQueryable<User> query = _db.Users;
        query = query.ApplySpecification(spec).Skip((page - 1) * pageSize).Take(pageSize);

        return await query.ToListAsync(cancellationToken: token);
    }

    public async Task<IdentityResult> AddAsync(User entity, string password)
    {
        return await _userManager.CreateAsync(entity, password);
    }

    public async Task<bool> CheckPasswordAsync(User entity, string password)
    {
        return await _userManager.CheckPasswordAsync(entity, password);
    }

    public async Task<IdentityResult> UpdateAsync(User entity)
    {
        return await _userManager.UpdateAsync(entity);
    }

    public async Task<IdentityResult> DeleteAsync(User entity)
    {
        return await _userManager.DeleteAsync(entity);
    }

    public async Task<IEnumerable<string>> GetRolesAsync(User user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> AddToRoleAsync(User user, string role)
    {
        return await _userManager.AddToRoleAsync(user, role);
    }

    public async Task<IdentityResult> RemoveFromRolesAsync(User user, string role)
    {
        return await _userManager.RemoveFromRoleAsync(user, role);
    }

    public async Task<int> GetTotalAsync()
    {
        return await _db.Users.CountAsync();
    }
    public async Task<User> AddUserTournament(User user, string tournamentId)
    {
        user.userTournaments.Add(new UserTournaments() { TournamentId = tournamentId, UserId = user.Id });
        await _db.SaveChangesAsync();
        return user;
    }
    public async Task<User> RemoveUserTournament(User user, string tournamentId)
    {
        var entity = await _db.Set<UserTournaments>().Where(c => c.UserId.Equals(user.Id) && c.TournamentId.Equals(tournamentId)).FirstOrDefaultAsync();
        var removedEntity = _db.Set<UserTournaments>().Remove(entity).Entity;
        await _db.SaveChangesAsync();
        return user;
    }
}