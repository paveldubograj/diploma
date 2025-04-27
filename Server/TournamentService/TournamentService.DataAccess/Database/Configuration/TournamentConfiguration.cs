using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentService.DataAccess.Entities;

namespace TournamentService.DataAccess.Database.Configuration;

public class TournamentConfiguration : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .HasMaxLength(60)
            .IsRequired();

        builder
            .Property(c => c.DisciplineId)
            .HasMaxLength(40)
            .IsRequired();

        builder
            .Property(c => c.OwnerId)
            .HasMaxLength(40)
            .IsRequired();
        
        builder
            .HasMany(c => c.Participants)
            .WithOne(d => d.Tournament)
            .HasForeignKey(d => d.TournamentId)
            .IsRequired();
        
        builder
            .Property(c => c.Status)
            .HasConversion<int>();

        builder
            .Property(c => c.Format)
            .HasConversion<int>();

        builder
            .Property(c => c.StartDate)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        builder
            .Property(c => c.EndDate)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    }
}
