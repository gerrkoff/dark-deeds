using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Entity.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace DarkDeeds.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : DeletableEntity
    {
        private readonly DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;
        }

        private DbSet<T> Entities => _context.Set<T>();
        
        
        public IQueryable<T> GetAll()
        {
            return Entities.Where(x => !x.IsDeleted).AsQueryable();
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
            return Entities.FindAsync(id);
        }
    }
}