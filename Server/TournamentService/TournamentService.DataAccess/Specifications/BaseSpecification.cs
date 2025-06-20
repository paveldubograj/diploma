using System;
using System.Linq.Expressions;
using TournamentService.DataAccess.Specifications.SpecSettings;

namespace TournamentService.DataAccess.Specifications;

public class BaseSpecification<T> : ISpecification<T>
{
    public BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public Expression<Func<T, bool>> Criteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; } = new ();
}
