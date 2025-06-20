using System;
using System.Linq.Expressions;
using NewsService.DataAccess.Entities;
using NewsService.DataAccess.Specifications.SpecSettings;

namespace NewsService.DataAccess.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public Expression<Func<T, bool>> Criteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; } = new ();
}
