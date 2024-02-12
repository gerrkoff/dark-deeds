using System.Linq.Expressions;

namespace DD.Shared.Data.Abstractions;

public interface IRepository<T>
    where T : BaseEntity
{
    IQueryable<T> GetAll();

    Task SaveAsync(T entity);

    Task SavePropertiesAsync(T entity, params Expression<Func<T, object>>[] properties);

    Task DeleteAsync(int id);

    Task DeleteAsync(T entity);

    Task<T?> GetByIdAsync(int id);
}
