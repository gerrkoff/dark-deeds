using DD.ServiceTask.Domain.Dto;

namespace DD.ServiceTask.Domain.Infrastructure;

public interface INotifierService
{
    Task TaskUpdated(TaskUpdatedDto updatedTasks);
}
