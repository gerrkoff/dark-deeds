using DD.TaskService.Domain.Dto;

namespace DD.TaskService.Domain.Infrastructure;

public interface INotifierService
{
    Task TaskUpdated(TaskUpdatedDto updatedTasks);
}
