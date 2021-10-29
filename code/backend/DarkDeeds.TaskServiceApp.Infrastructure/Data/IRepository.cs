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
        Task<T> GetByIdAsync(string uid);
        Task<IList<T>> GetBySpecAsync(ISpecification<T> spec);
        Task<IList<T>> GetListAsync();

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task UpsertAsync(T entity);
        Task UpdatePropertiesAsync(T entity, params Expression<Func<T, object>>[] properties);
        
        Task DeleteAsync(string uid);
        Task DeleteHardAsync(string uid);
    }
}