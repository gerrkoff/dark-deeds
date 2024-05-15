using DD.Shared.Details.Abstractions.Dto;

namespace DD.Shared.Details.Abstractions;

public interface ITaskServiceSubscriber
{
    Task TasksUpdated(TasksUpdatedDto tasksUpdated);
}
