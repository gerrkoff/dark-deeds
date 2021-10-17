using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models.Base;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Data
{
    public interface IRepository<T> : IRepositoryNonDeletable<T> where T : DeletableEntity
    {
        Task DeleteAsync(int id);
        Task DeleteAsync(T entity);
    }
}