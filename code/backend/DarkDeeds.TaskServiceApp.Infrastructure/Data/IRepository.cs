using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models.Abstractions;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Data
{
    public interface IRepository<T>
        where T: Entity
    {
        Task<T> GetByIdAsync(string uid);
        Task<IList<T>> GetBySpecAsync(ISpecification<T> spec);
        // TODO!
        Task<IList<T>> GetListAsync(); 

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // TODO: split on update & insert
        Task UpsertAsync(T entity);
        // TODO! use
        Task<(bool, T)> TryUpdateVersionAsync(T entity);
        Task<(bool, T)> TryUpdateVersionPropsAsync(T entity, params Expression<Func<T, object>>[] properties);

        Task DeleteAsync(string uid);
        Task DeleteHardAsync(string uid);
    }
}