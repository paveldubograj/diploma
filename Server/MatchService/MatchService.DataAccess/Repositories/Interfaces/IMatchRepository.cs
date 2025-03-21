using System;
using MatchService.DataAccess.Entities;
using MatchService.DataAccess.Specifications;

namespace MatchService.DataAccess.Repositories.Interfaces;

public interface IMatchRepository
{
    Task<Match> GetByIdAsync(string id);
    Task<List<Match>> GetTournamentStructureAsync(string TournamentId);
    Task<List<Match>> GetAsync(int page, int pageSize);
    Task<Match> AddAsync(Match Match);
    Task<Match> DeleteAsync(Match Match);
    Task<Match> UpdateAsync(Match Match);
    Task<IEnumerable<Match>> GetBySpecificationAsync(MatchSpecification spec, int page, int pageSize, CancellationToken token = default);
}
