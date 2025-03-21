using System;
using DisciplineService.BusinessLogic.Models;

namespace DisciplineService.BusinessLogic.Services.Interfaces;

public interface IDisciplineService
{
    public Task<List<DisciplineCleanDto>> GetAllAsync();
    public Task<DisciplineCleanDto> DeleteAsync(string id);
    public Task<DisciplineDto> UpdateAsync(string id, DisciplineDto disciplineDto);
    public Task<DisciplineDto> AddAsync(DisciplineDto disciplineDto);
    public Task<DisciplineDto> GetByIdAsync(string id);
}
