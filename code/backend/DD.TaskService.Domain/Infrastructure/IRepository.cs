using System.Linq.Expressions;
using DD.TaskService.Domain.Entities.Abstractions;
using DD.TaskService.Domain.Specifications;

namespace DD.TaskService.Domain.Infrastructure;

public interface IRepository<T> where T: Entity
{
    Task<T> GetByIdAsync(string uid);
    Task<IList<T>> GetBySpecAsync(ISpecification<T> spec);

    Task<bool> AnyAsync(ISpecification<T> spec);

    // TODO: split on update & insert
    Task UpsertAsync(T entity);
    Task<(bool, T)> TryUpdateVersionAsync(T entity);
    Task<(bool, T)> TryUpdateVersionPropsAsync(T entity, params Expression<Func<T, object>>[] properties);

    Task<bool> DeleteAsync(string uid);
    Task<bool> DeleteHardAsync(string uid);
}
