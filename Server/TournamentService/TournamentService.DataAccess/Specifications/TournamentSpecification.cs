using System;
using System.Linq.Expressions;
using TournamentService.DataAccess.Entities;

namespace TournamentService.DataAccess.Specifications;

public class TournamentSpecification : BaseSpecification<Tournament>
{
    public TournamentSpecification(Expression<Func<Tournament, bool>> criteria) : base(criteria)
    {
        
    }
}
