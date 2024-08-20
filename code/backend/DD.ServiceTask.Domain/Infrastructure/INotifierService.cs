using DD.Shared.Details.Abstractions.Dto;

namespace DD.ServiceTask.Domain.Infrastructure;

public interface INotifierService
{
    Task TaskUpdated(TasksUpdatedDto updatedTasks);
}
