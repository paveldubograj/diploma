using System;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Specifications;

namespace TournamentService.DataAccess.Repositories.Interfaces;

public interface ITournamentRepository
{
    Task<Tournament> GetByIdAsync(string id);
    Task<List<Tournament>> GetAsync(int page, int pageSize);
    Task<Tournament> AddAsync(Tournament tournament);
    Task<Tournament> DeleteAsync(Tournament tournament);
    Task<Tournament> UpdateAsync(Tournament tournament);
    Task<IEnumerable<Tournament>> GetBySpecificationAsync(TournamentSpecification spec1, int page, int pageSize, CancellationToken token = default);
}
