using System;
using DarkDeeds.TaskServiceApp.Entities.Models;

namespace DarkDeeds.TaskServiceApp.Services.Specifications
{
    public interface ITaskSpecification : IUserOwnedSpecification<TaskEntity, ITaskSpecification>
    {
        ITaskSpecification FilterActual(DateTime from);
    }
}