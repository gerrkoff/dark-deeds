using System.Linq.Expressions;

namespace DD.ServiceTask.Domain.Specifications;

public interface ISpecification<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}

public abstract class Specification<T> : ISpecification<T>
{
    protected IList<Expression<Func<T, bool>>> Filters { get; } = [];

    public IQueryable<T> Apply(IQueryable<T> query)
    {
        foreach (var filter in Filters)
        {
            query = query.Where(filter);
        }

        return query;
    }
}
