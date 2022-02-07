using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DarkDeeds.Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.Backend.Data
{
    public class Repository<T> : IRepositoryNonDeletable<T>
        where T : BaseEntity
    {
        private readonly DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;
        }

        private DbSet<T> Entities => _context.Set<T>();
        
        
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
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SavePropertiesAsync(T entity, params Expression<Func<T, object>>[] properties)
        {
            if (entity.Id == 0)
            {
                return;
            }

            Entities.Attach(entity);

            IList<string> propertiesToSave = properties
                .Select(x => _context.Entry(entity).Property(x))
                .Select(x => x.Metadata.GetFieldName())
                .ToList();

            foreach (var property in _context.Entry(entity).Properties)
            {
                property.IsModified = propertiesToSave.Contains(property.Metadata.GetFieldName());
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(int id)
        {
            var entity = Activator.CreateInstance<T>();
            entity.Id = id;			
            Entities.Attach(entity);
            Entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(T entity)
        {
            Entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<T> GetByIdAsync(int id)
        {
            return Entities.FindAsync(id).AsTask();
        }
    }
}