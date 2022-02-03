using System;
using DarkDeeds.ServiceTask.Entities.Models;

namespace DarkDeeds.ServiceTask.Services.Specifications
{
    public interface ITaskSpecification : IEntitySpecification<TaskEntity, ITaskSpecification>, 
        IUserOwnedSpecification<TaskEntity, ITaskSpecification>
    {
        ITaskSpecification FilterActual(DateTime from);
        ITaskSpecification FilterDateInterval(DateTime from, DateTime to);
    }
}