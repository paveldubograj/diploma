using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.DataAccess.DataBase;
using UserService.DataAccess.Entities;
using UserService.DataAccess.Repositories.Interfaces;
using UserService.DataAccess.Specifications;
using UserService.DataAccess.Specifications.SpecSettings;

namespace UserService.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private UsersContext db;
    private UserManager<User> _userManager;

    public UserRepository(UsersContext db, UserManager<User> userManager)
    {
        this.db = db;
        _userManager = userManager;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetBySpecAsync(UserSpecification spec, CancellationToken token = default)
    {
        IQueryable<User> query = db.Users;
        query = query.ApplySpecification(spec);
        
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
}