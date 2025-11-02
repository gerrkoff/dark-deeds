using AutoMapper;
using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Infrastructure;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.ServiceTask.Domain.Specifications;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.Extensions.Logging;

namespace DD.ServiceTask.Domain.Services;

public interface ITaskService
{
    /// <summary>
    /// Get all tasks, that:
    /// <para>1. fits current period or in future.</para>
    /// <para>2. has no date.</para>
    /// <para>3. expired, but not completed.</para>
    /// </summary>
    /// <param name="userId">Task's user id.</param>
    /// <param name="from">Current period start, UTC expected.</param>
    /// <returns>Tasks.</returns>
    Task<IEnumerable<TaskDto>> LoadActualTasksAsync(string userId, DateTime from);

    /// <summary>
    /// Get all tasks, filtered by date.
    /// </summary>
    /// <param name="userId">Task's user id.</param>
    /// <param name="from">Period start, included, UTC expected.</param>
    /// <param name="till">Period end, not included, UTC expected.</param>
    /// <returns>Tasks.</returns>
    Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime till);

    Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId, string? clientId);

    Task<IEnumerable<TaskDto>> UpdateTasksAsync(ICollection<TaskUpdateDto> updates, string userId, string? clientId);
}

public class TaskService(
    ITaskRepository tasksRepository,
    ILogger<TaskService> logger,
    IMapper mapper,
    INotifierService notifierService,
    ISpecificationFactory specFactory)
    : ITaskService
{
    public async Task<IEnumerable<TaskDto>> LoadActualTasksAsync(string userId, DateTime from)
    {
        var spec = specFactory.Create<ITaskSpecification, TaskEntity>()
            .FilterUserOwned(userId)
            .FilterActual(from)
            .FilterNotDeletedEarlier(from.AddDays(-7));

        var tasks = await tasksRepository.GetBySpecAsync(spec);

        return mapper.Map<IList<TaskDto>>(tasks);
    }

    public async Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime till)
    {
        var spec = specFactory.Create<ITaskSpecification, TaskEntity>()
            .FilterUserOwned(userId)
            .FilterDateInterval(from, till)
            .FilterNotDeleted();

        var tasks = await tasksRepository.GetBySpecAsync(spec);

        return mapper.Map<IList<TaskDto>>(tasks);
    }

    public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId, string? clientId)
    {
        var savedTasks = new List<TaskDto>();

        foreach (var taskDto in tasks)
        {
            var savedTask = await SaveTaskByUidAsync(taskDto, userId);
            if (savedTask != null)
                savedTasks.Add(savedTask);
        }

        if (savedTasks.Count > 0)
        {
            await notifierService.TaskUpdated(new TasksUpdatedDto
            {
                Tasks = savedTasks,
                UserId = userId,
                ClientId = clientId,
            });
        }

        return savedTasks;
    }

    public async Task<IEnumerable<TaskDto>> UpdateTasksAsync(ICollection<TaskUpdateDto> updates, string userId, string? clientId)
    {
        var updatedTasks = new List<TaskDto>();

        foreach (var update in updates)
        {
            var entity = await tasksRepository.GetByIdAsync(update.Uid);

            if (entity == null)
            {
                Log.TriedToUpdateNonExistingTask(logger, update.Uid);
                continue;
            }

            if (entity.DeletedAt != null)
            {
                Log.TriedToUpdateDeletedTask(logger, update.Uid);
                continue;
            }

            if (!string.Equals(entity.UserId, userId, StringComparison.Ordinal))
            {
                Log.TriedToUpdateForeignTask(logger, update.Uid, userId);
                continue;
            }

            entity.Order = update.Order;
            var (success, _) = await tasksRepository.TryUpdateVersionAsync(entity);
            if (!success)
            {
                Log.UpdateTaskVersionConflict(logger, update.Uid);
                continue;
            }

            updatedTasks.Add(mapper.Map<TaskDto>(entity));
        }

        if (updatedTasks.Count > 0)
        {
            await notifierService.TaskUpdated(new TasksUpdatedDto
            {
                Tasks = updatedTasks,
                UserId = userId,
                ClientId = clientId,
            });
        }

        return updatedTasks;
    }

    private async Task<TaskDto?> SaveTaskByUidAsync(TaskDto taskToSave, string userId)
    {
        var entity = await tasksRepository.GetByIdAsync(taskToSave.Uid);

        if (entity != null && !string.Equals(entity.UserId, userId, StringComparison.Ordinal))
        {
            Log.TriedToUpdateForeignTask(logger, taskToSave.Uid, userId);
            return null;
        }

        if (taskToSave.Deleted)
        {
            if (entity == null)
            {
                Log.TriedToDeleteNonExistingTask(logger, taskToSave.Uid);
                taskToSave.Version++;
                return taskToSave;
            }

            entity.DeletedAt = DateTimeOffset.UtcNow;
            var (success, _) = await tasksRepository.TryUpdateVersionAsync(entity);
            if (!success)
                return null;
        }
        else if (entity == null)
        {
            entity = mapper.Map<TaskEntity>(taskToSave);
            entity.UserId = userId;
            entity.Version = 1;
            await tasksRepository.UpsertAsync(entity);
        }
        else
        {
            entity = mapper.Map(taskToSave, entity);
            var (success, _) = await tasksRepository.TryUpdateVersionAsync(entity);
            if (!success)
                return null;
        }

        return mapper.Map<TaskDto>(entity);
    }
}
