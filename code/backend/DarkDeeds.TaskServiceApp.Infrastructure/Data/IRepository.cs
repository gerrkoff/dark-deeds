using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Data
{
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(string id);
        Task<IList<T>> GetBySpecAsync(ISpecification<T> spec);
        Task<IList<T>> GetListAsync();
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task SaveAsync(T entity);
        Task DeleteAsync(string id);
        Task SaveRecurrences(T entity);
        
        // TODO: save props
        // TODO: hard delete
    }
}