using System;
using DisciplineService.DataAccess.Entities;

namespace DisciplineService.DataAccess.Repositories.Interfaces;

public interface IDisciplineRepository
{
    Task<Discipline> AddAsync(Discipline discipline);
    Task<List<Discipline>> GetAllAsync();
    Task<Discipline> UpdateAsync(Discipline discipline);
    Task<Discipline> DeleteAsync(Discipline discipline);
    Task<Discipline?> GetByIdAsync(string id);
}
