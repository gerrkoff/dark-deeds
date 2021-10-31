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

        Task<bool> AnyAsync(ISpecification<T> spec);

        // TODO: split on update & insert
        Task UpsertAsync(T entity);
        Task<(bool, T)> TryUpdateVersionAsync(T entity);
        Task<(bool, T)> TryUpdateVersionPropsAsync(T entity, params Expression<Func<T, object>>[] properties);

        Task<bool> DeleteAsync(string uid);
        Task<bool> DeleteHardAsync(string uid);
    }
}