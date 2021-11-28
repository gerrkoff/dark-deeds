using System;
using DarkDeeds.TaskServiceApp.Entities.Models;

namespace DarkDeeds.TaskServiceApp.Services.Specifications
{
    public interface ITaskSpecification : IEntitySpecification<TaskEntity, ITaskSpecification>, 
        IUserOwnedSpecification<TaskEntity, ITaskSpecification>
    {
        ITaskSpecification FilterActual(DateTime from);
        ITaskSpecification FilterDateInterval(DateTime from, DateTime to);
    }
}