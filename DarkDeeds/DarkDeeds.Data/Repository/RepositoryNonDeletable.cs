using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DarkDeeds.Entities.Models.Base;
using DarkDeeds.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.Data.Repository
{
    public class RepositoryNonDeletable<T> : IRepositoryNonDeletable<T>
        where T : BaseEntity
    {
        protected readonly DbContext Context;

        public RepositoryNonDeletable(DbContext context)
        {
            Context = context;
        }

        private DbSet<T> Entities => Context.Set<T>();
        
        
        public virtual IQueryable<T> GetAll()
        {
            return Entities.AsQueryable();
        }

        public async Task SaveAsync(T entity)
        {
            if (entity.Id == 0)
            {
                Entities.Add(entity);
            }
            else
            {
                Entities.Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
            }

            await Context.SaveChangesAsync();
        }

        public async Task SavePropertiesAsync(T entity, params Expression<Func<T, object>>[] properties)
        {
            if (entity.Id == 0)
            {
                return;
            }

            Entities.Attach(entity);

            IList<string> propertiesToSave = properties
                .Select(x => Context.Entry(entity).Property(x))
                .Select(x => x.Metadata.GetFieldName())
                .ToList();

            foreach (var property in Context.Entry(entity).Properties)
            {
                property.IsModified = propertiesToSave.Contains(property.Metadata.GetFieldName());
            }

            await Context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(int id)
        {
            var entity = Activator.CreateInstance<T>();
            entity.Id = id;			
            Entities.Attach(entity);
            Entities.Remove(entity);
            await Context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(T entity)
        {
            Entities.Remove(entity);
            await Context.SaveChangesAsync();
        }

        public Task<T> GetByIdAsync(int id)
        {
            return Entities.FindAsync(id).AsTask();
        }
    }
}