using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models.Interfaces;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Data
{
    public interface IRepository<T>
        where T: IWithId
    {
        Task<T> GetByIdAsync(string id);
        Task<IList<T>> GetBySpecAsync(ISpecification<T> spec);
        Task<IList<T>> GetListAsync();
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task SaveAsync(T entity);
        Task DeleteAsync(string id);
        // TODO: split on insert & update
        Task SaveRecurrences(T entity);
        
        // TODO: save props
        // TODO: hard delete
    }
}