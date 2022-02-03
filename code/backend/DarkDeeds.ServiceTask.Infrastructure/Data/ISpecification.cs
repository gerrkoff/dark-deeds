using System.Linq;

namespace DarkDeeds.ServiceTask.Infrastructure.Data
{
    public interface ISpecification<T>
    {
        IQueryable<T> Apply(IQueryable<T> query);
    }
}