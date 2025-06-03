using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UserService.DataAccess.DataBase.Configurations;
using UserService.DataAccess.Entities;

namespace UserService.DataAccess.DataBase;

public class UsersContext : IdentityDbContext<User>
{
    //public string str = "Host=localhost;Port=5432;Database=Diploma.UserService;Username=pavel;Password=1234";
    public DbSet<User> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<UserTournaments> UserTournaments { get; set; }
    
    public UsersContext(DbContextOptions<UsersContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseNpgsql(str);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}