using System;
using Microsoft.EntityFrameworkCore;
using NewsService.DataAccess.Database.Configuration;
using NewsService.DataAccess.Entities;

namespace NewsService.DataAccess.Database;

public class NewsContext : DbContext
{
    public string str = "Host=localhost;Port=5432;Database=Diploma.NewsService;Username=pavel;Password=1234";
    public DbSet<News> News {get; set;}
    public DbSet<Tag> Tags {get; set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(str);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NewsConfiguration).Assembly);
    }
}
