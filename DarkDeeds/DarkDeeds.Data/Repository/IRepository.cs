using System.Threading.Tasks;
using DarkDeeds.Entities.Models.Base;

namespace DarkDeeds.Data.Repository
{
    public interface IRepository<T> : IRepositoryNonDeletable<T>
        where T : DeletableEntity
    {
        Task DeleteAsync(int id);
        Task DeleteAsync(T entity);
    }
}