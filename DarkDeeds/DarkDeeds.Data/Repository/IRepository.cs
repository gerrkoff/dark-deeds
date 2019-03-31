using System.Threading.Tasks;
using DarkDeeds.Data.Entity.Base;

namespace DarkDeeds.Data.Repository
{
    public interface IRepository<T> : IRepositoryNonDeletable<T>
        where T : DeletableEntity
    {
        Task DeleteAsync(int id);
        Task DeleteAsync(T entity);
    }
}