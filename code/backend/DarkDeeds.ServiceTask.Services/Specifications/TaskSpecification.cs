using System;
using DarkDeeds.ServiceTask.Entities.Enums;
using DarkDeeds.ServiceTask.Entities.Models;

namespace DarkDeeds.ServiceTask.Services.Specifications
{
    class TaskSpecification : UserOwnedSpecification<TaskEntity, ITaskSpecification>, ITaskSpecification
    {
        public ITaskSpecification FilterActual(DateTime from)
        {
            Filters.Add(x => !x.IsCompleted && x.Type != TaskTypeEnum.Additional ||
                             !x.Date.HasValue ||
                             x.Date >= from);
            return this;
        }
        
        public ITaskSpecification FilterDateInterval(DateTime from, DateTime to)
        {
            Filters.Add(x => x.Date.HasValue && x.Date >= from && x.Date < to);
            return this;
        }
    }
}
