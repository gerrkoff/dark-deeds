using System.Linq.Expressions;
using DD.Shared.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DD.Shared.Data;

class Repository<T>(DbContext context) : IRepository<T>
    where T : BaseEntity
{
    private DbSet<T> Entities => context.Set<T>();


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
            context.Entry(entity).State = EntityState.Modified;
        }

        await context.SaveChangesAsync();
    }

    public async Task SavePropertiesAsync(T entity, params Expression<Func<T, object>>[] properties)
    {
        if (entity.Id == 0)
        {
            return;
        }

        Entities.Attach(entity);

        IList<string> propertiesToSave = properties
            .Select(x => context.Entry(entity).Property(x))
            .Select(x => x.Metadata.GetFieldName())
            .ToList();

        foreach (var property in context.Entry(entity).Properties)
        {
            property.IsModified = propertiesToSave.Contains(property.Metadata.GetFieldName());
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = Activator.CreateInstance<T>();
        entity.Id = id;
        Entities.Attach(entity);
        Entities.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        Entities.Remove(entity);
        await context.SaveChangesAsync();
    }

    public Task<T> GetByIdAsync(int id)
    {
        return Entities.FindAsync(id).AsTask();
    }
}
