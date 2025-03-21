using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsService.DataAccess.Entities;

namespace NewsService.DataAccess.Database.Configuration;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Title)
            .HasMaxLength(60)
            .IsRequired();

        builder
            .Property(c => c.Content)
            .HasColumnType("TEXT")
            .IsRequired();

        builder
            .Property(c => c.CategoryId)
            .HasMaxLength(40)
            .IsRequired();

        builder
            .Property(c => c.AuthorId)
            .HasMaxLength(40)
            .IsRequired();
        
        builder
            .HasMany(c => c.Tags)
            .WithMany(d => d.News)
            .UsingEntity(j => j.ToTable("NewsTags"));
    }
}
