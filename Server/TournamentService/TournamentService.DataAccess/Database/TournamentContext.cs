using System;
using Microsoft.EntityFrameworkCore;
using TournamentService.DataAccess.Database.Configuration;
using TournamentService.DataAccess.Entities;

namespace TournamentService.DataAccess.Database;

public class TournamentContext : DbContext
{
    public string str = "Host=localhost;Port=5432;Database=Diploma.TournamentService;Username=pavel;Password=1234";
    public DbSet<Tournament> Tournaments {get; set;}
    public DbSet<Participant> Participants {get; set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(str);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TournamentConfiguration).Assembly);
    }
}
