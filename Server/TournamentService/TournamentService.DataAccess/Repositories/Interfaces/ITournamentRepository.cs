using System;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Specifications;
using TournamentService.Shared.Enums;

namespace TournamentService.DataAccess.Repositories.Interfaces;

public interface ITournamentRepository
{
    Task<Tournament?> GetByIdAsync(string id);
    Tournament GetById(string id);
    Task<List<Tournament>> GetAsync(int page, int pageSize);
    Task<int> GetTotalAsync();
    Participant GetParticipantById(string id);
    Task<Tournament> AddAsync(Tournament tournament);
    Task<Tournament> DeleteAsync(Tournament tournament);
    Task<Tournament> UpdateAsync(Tournament tournament);
    Task<bool> IsRegistrationAllowed(string tournamentId);
    Task<IEnumerable<Tournament>> GetBySpecificationAsync(TournamentSpecification spec1, TournamentSortOptions? options, int page, int pageSize, CancellationToken token = default);
}
