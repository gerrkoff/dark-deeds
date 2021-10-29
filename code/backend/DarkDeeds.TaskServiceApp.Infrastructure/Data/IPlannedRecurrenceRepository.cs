using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Data
{
    public interface IPlannedRecurrenceRepository
    {
        IQueryable<PlannedRecurrenceEntity> GetAll();
        Task<PlannedRecurrenceEntity> GetByIdAsync(string id);
        Task<IList<PlannedRecurrenceEntity>> GetListAsync();
        Task<bool> AnyAsync(Expression<Func<PlannedRecurrenceEntity, bool>> predicate);
        Task SaveAsync(PlannedRecurrenceEntity entity);
        Task DeleteAsync(string id);
        Task SaveRecurrences(PlannedRecurrenceEntity entity);
    }
}