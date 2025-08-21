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
        var weekEnd = from.AddDays(7);
        Filters.Add(x =>
            // 1. Not completed and not expired (for non-weekly) OR weekly inside current week
            (!x.IsCompleted && x.Type != TaskType.Additional && x.Type != TaskType.Routine &&
                (x.Type != TaskType.Weekly || (x.Date.HasValue && x.Date.Value >= from && x.Date.Value < weekEnd)))
            // 2. Or has no date (always included)
            || !x.Date.HasValue
            // 3. Or scheduled today or in future (any type) beyond the from boundary
            || x.Date >= from);
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
