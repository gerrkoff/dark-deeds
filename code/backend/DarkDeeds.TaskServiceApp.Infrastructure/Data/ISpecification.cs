using System.Linq;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Data
{
    public interface ISpecification<T>
    {
        IQueryable<T> Apply(IQueryable<T> query);
    }
}