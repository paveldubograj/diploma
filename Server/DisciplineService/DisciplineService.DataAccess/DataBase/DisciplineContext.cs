using System;
using DisciplineService.DataAccess.DataBase.Configurations;
using DisciplineService.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DisciplineService.DataAccess.DataBase;

public class DisciplineContext : DbContext
{
    public string str = "Host=localhost;Port=5432;Database=Diploma.DisciplineService;Username=pavel;Password=1234";
    public DbSet<Discipline> disciplines {get; set;}
    public DisciplineContext(DbContextOptions<DisciplineContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(str);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DisciplineConfiguration).Assembly);
    }
}
