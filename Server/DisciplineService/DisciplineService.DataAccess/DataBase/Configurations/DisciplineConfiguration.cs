using System;
using DisciplineService.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DisciplineService.DataAccess.DataBase.Configurations;

public class DisciplineConfiguration : IEntityTypeConfiguration<Discipline>
{
    public void Configure(EntityTypeBuilder<Discipline> builder)
    {
        builder
            .HasKey(t => t.Id);
        
        builder
            .Property(t => t.Name)
            .HasMaxLength(40)
            .IsRequired();
    }
}
