using System;
using System.Text.RegularExpressions;
using MatchService.DataAccess.Database.Configuration;
using MatchService.DataAccess.Entities;
using MatchService.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Match = MatchService.DataAccess.Entities.Match;

namespace MatchService.DataAccess.Database;

public class MatchContext : DbContext
{
    public string str = "Host=localhost;Port=5432;Database=Diploma.MatchService;Username=pavel;Password=1234";
    public DbSet<Match> Matches {get; set;}
    public MatchContext(DbContextOptions<MatchContext> options) : base(options){}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(str);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MatchConfiguration).Assembly);
    }
}
