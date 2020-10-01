using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Entities.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.Data.Repository
{
    public class Repository<T> : RepositoryNonDeletable<T>, IRepository<T>
        where T : DeletableEntity
    {
        public Repository(DbContext context) : base(context)
        {
        }

        private DbSet<T> Entities => Context.Set<T>();
        
        
        public override IQueryable<T> GetAll()
        {
            return Entities.Where(x => !x.IsDeleted).AsQueryable();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = Activator.CreateInstance<T>();
            entity.Id = id;
            entity.IsDeleted = true;
            await SavePropertiesAsync(entity, x => x.IsDeleted);
        }

        public async Task DeleteAsync(T entity)
        {
            entity.IsDeleted = true;
            await SavePropertiesAsync(entity, x => x.IsDeleted);
        }
    }
}