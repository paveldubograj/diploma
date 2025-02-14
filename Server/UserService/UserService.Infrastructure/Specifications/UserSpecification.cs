using System;
using System.Linq.Expressions;
using UserService.DataAccess.Entities;

namespace UserService.DataAccess.Specifications;

public class UserSpecification : BaseSpecification<User>
{
    public UserSpecification(Expression<Func<User, bool>> criteria) : base(criteria)
    {
    }
}
