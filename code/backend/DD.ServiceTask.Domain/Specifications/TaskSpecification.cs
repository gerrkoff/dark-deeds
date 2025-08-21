using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Entities.Enums;

namespace DD.ServiceTask.Domain.Specifications;

public interface ITaskSpecification : IEntitySpecification<TaskEntity, ITaskSpecification>,
    IUserOwnedSpecification<TaskEntity, ITaskSpecification>
{
    ITaskSpecification FilterActual(DateTime from);

    ITaskSpecification FilterDateInterval(DateTime from, DateTime till);

    ITaskSpecification FilterNotDeletedEarlier(DateTime deletedAt);
}

public class TaskSpecification : UserOwnedSpecification<TaskEntity, ITaskSpecification>, ITaskSpecification
{
    // important
    public ITaskSpecification FilterActual(DateTime from)
    {
        Filters.Add(x => (!x.IsCompleted && x.Type != TaskType.Additional && x.Type != TaskType.Routine) ||
                        !x.Date.HasValue ||
                        x.Date >= from);
        return this;
    }

    public ITaskSpecification FilterDateInterval(DateTime from, DateTime till)
    {
        Filters.Add(x => x.Date.HasValue && x.Date >= from && x.Date < till);
        return this;
    }

    public ITaskSpecification FilterNotDeletedEarlier(DateTime deletedAt)
    {
        Filters.Add(x => x.DeletedAt == null || x.DeletedAt >= deletedAt);
        return this;
    }
}
