using System;
using MatchService.DataAccess.Entities;
using MatchService.DataAccess.Specifications;
using MatchService.Shared.Enums;

namespace MatchService.DataAccess.Repositories.Interfaces;

public interface IMatchRepository
{
    Task<Match> GetByIdAsync(string id);
    Task<List<Match>> GetTournamentStructureAsync(string TournamentId);
    Task<List<Match>> GetAsync(int page, int pageSize);
    Task<Match> AddAsync(Match match);
    Task<List<Match>> AddRange(List<Match> matches);
    Task<Match> DeleteAsync(Match match);
    Task<Match> UpdateAsync(Match match);
    Task<IEnumerable<Match>> GetBySpecificationAsync(MatchSpecification spec, SortOptions? options, int page, int pageSize, CancellationToken token = default);
    Task<Match> GetOneBySpecificationAsync(MatchSpecification spec, CancellationToken token = default);
    Task<int> GetTotalAsync();
}
