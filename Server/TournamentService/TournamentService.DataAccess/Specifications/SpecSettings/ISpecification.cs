using System;
using System.Linq.Expressions;

namespace TournamentService.DataAccess.Specifications.SpecSettings;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
}
