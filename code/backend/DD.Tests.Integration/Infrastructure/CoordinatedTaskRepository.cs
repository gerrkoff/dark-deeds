using System.Linq.Expressions;
using DD.ServiceTask.Details.Data;
using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.ServiceTask.Domain.Specifications;

namespace DD.Tests.Integration.Infrastructure;

internal sealed class CoordinatedTaskRepository(
    TaskRepository repository,
    TaskUpdateBarrier updateBarrier)
    : ITaskRepository
{
    public Task<TaskEntity?> GetByIdAsync(string uid)
    {
        return repository.GetByIdAsync(uid);
    }

    public Task<IList<TaskEntity>> GetBySpecAsync(ISpecification<TaskEntity> spec)
    {
        return repository.GetBySpecAsync(spec);
    }

    public Task<bool> AnyAsync(ISpecification<TaskEntity> spec)
    {
        return repository.AnyAsync(spec);
    }

    public Task UpsertAsync(TaskEntity entity)
    {
        return repository.UpsertAsync(entity);
    }

    public async Task<(bool Success, TaskEntity? CurrentEntity)> TryUpdateVersionAsync(
        TaskEntity entity)
    {
        await updateBarrier.WaitAsync(entity.Uid);
        return await repository.TryUpdateVersionAsync(entity);
    }

    public Task<(bool Success, TaskEntity? CurrentEntity)> TryUpdateVersionPropsAsync(
        TaskEntity entity,
        params Expression<Func<TaskEntity, object>>[] properties)
    {
        return repository.TryUpdateVersionPropsAsync(entity, properties);
    }

    public Task<bool> DeleteAsync(string uid)
    {
        return repository.DeleteAsync(uid);
    }

    public Task<bool> DeleteHardAsync(string uid)
    {
        return repository.DeleteHardAsync(uid);
    }
}
