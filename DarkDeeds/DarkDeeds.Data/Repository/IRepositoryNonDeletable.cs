using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity.Base;

namespace DarkDeeds.Data.Repository
{
    public interface IRepositoryNonDeletable<T> 
        where T : BaseEntity
    {
        IQueryable<T> GetAll();
        Task SaveAsync(T entity);
        Task SavePropertiesAsync(T entity, params Expression<Func<T, object>>[] properties);
        Task DeleteHardAsync(int id);
        Task DeleteHardAsync(T entity);
        Task<T> GetByIdAsync(int id);
    }
}