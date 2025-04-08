using System;
using MatchService.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MatchService.DataAccess.Database.Configuration;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.WinnerId)
            .HasMaxLength(40);

        builder
            .Property(c => c.Participant1Id)
            .HasMaxLength(40)
            .IsRequired();

        builder
            .Property(c => c.Participant2Id)
            .HasMaxLength(40)
            .IsRequired();

        builder
            .Property(c => c.TournamentId)
            .HasMaxLength(40)
            .IsRequired();

        builder
            .Property(c => c.OwnerId)
            .HasMaxLength(40)
            .IsRequired();

        builder
            .Property(c => c.Status)
            .HasConversion<int>();
    }
}
