using System;
using System.Linq.Expressions;
using TournamentService.DataAccess.Entities;
using TournamentService.DataAccess.Specifications.SpecSettings;

namespace TournamentService.DataAccess.Specifications;

public class TournamentSpecification : BaseSpecification<Tournament>
{
    public TournamentSpecification(Expression<Func<Tournament, bool>> criteria) : base(criteria)
    {
        
    }

    public static TournamentSpecification FilterTournaments(string? SearchString, string? CategoryId, int? Status, int? Format, DateTime? StartTime, DateTime? EndTime){
        var predicate = PredicateBuilder.True<Tournament>();
        if(!string.IsNullOrEmpty(CategoryId)){
            predicate = predicate.And(tournament => tournament.DisciplineId.Equals(CategoryId));
        }
        if(StartTime is not null){
            predicate = predicate.And(tournament => tournament.StartDate >= StartTime);
        }
        if(EndTime is not null){
            predicate = predicate.And(tournament => tournament.EndDate <= EndTime);
        }
        if(Status is not null){
            predicate = predicate.And(tournament => ((int)tournament.Status) == Status);
        }
        if(Format is not null){
            predicate = predicate.And(tournament => ((int)tournament.Format) == Format);
        }
        if(SearchString is not null){
            predicate = predicate.And(tournament => tournament.Name.Contains(SearchString));
        }
        return new TournamentSpecification(predicate);
    }
}
