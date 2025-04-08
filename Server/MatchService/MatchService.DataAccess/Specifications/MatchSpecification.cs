using System;
using System.Linq.Expressions;
using MatchService.DataAccess.Entities;
using MatchService.DataAccess.Specifications.SpecSettings;

namespace MatchService.DataAccess.Specifications;

public class MatchSpecification : BaseSpecification<Match>
{
    public MatchSpecification(Expression<Func<Match, bool>> criteria) : base(criteria)
    {
        
    }
    public static MatchSpecification FilterMatch(string CategoryId, DateTime? StartTime, DateTime? EndTime, string TournamentId, string Status){
        var predicate = PredicateBuilder.True<Match>();
        if(!string.IsNullOrEmpty(CategoryId)){
            predicate = predicate.And(match => match.CategoryId.Equals(CategoryId));
        }
        if(StartTime is not null){
            predicate = predicate.And(match => match.StartTime >= StartTime);
        }
        if(StartTime is not null){
            predicate = predicate.And(match => match.EndTime <= EndTime);
        }
        if(!string.IsNullOrEmpty(TournamentId)){
            predicate = predicate.And(match => match.TournamentId.Equals(TournamentId));
        }
        if(!string.IsNullOrEmpty(Status)){
            predicate = predicate.And(match => match.Status.Equals(Status));
        }
        return new MatchSpecification(predicate);
    }

    public static MatchSpecification FindTournamentRound(string TournamentId, string round){
        var predicate = PredicateBuilder.True<Match>();
        predicate = predicate.And(match => match.TournamentId.Equals(TournamentId));
        predicate = predicate.And(match => match.Round.Equals(round));
        return new MatchSpecification(predicate);
    }
}
