using System;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models;

namespace DarkDeeds.TaskServiceApp.Services.Specifications
{
    public class TaskSpecification : UserOwnedSpecification<TaskEntity, ITaskSpecification>, ITaskSpecification
    {
        public ITaskSpecification FilterActual(DateTime from)
        {
            Filters.Add(x => !x.IsCompleted && x.Type != TaskTypeEnum.Additional ||
                             !x.Date.HasValue ||
                             x.Date >= from);
            return this;
        }
    }
}
