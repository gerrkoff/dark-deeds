using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models.Interfaces;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Data
{
    public interface IRepository<T>
        where T: IEntity
    {
        // TODO! id rename
        // TODO! save rename
        Task<T> GetByIdAsync(string id);
        Task<IList<T>> GetBySpecAsync(ISpecification<T> spec);
        Task<IList<T>> GetListAsync();

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // TODO: split on insert & update
        Task SaveAsync(T entity);
        Task SavePropertiesAsync(T entity, params Expression<Func<T, object>>[] properties);
        
        Task DeleteAsync(string id);
        Task DeleteHardAsync(string id);
    }
}