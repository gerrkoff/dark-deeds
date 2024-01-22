using DD.TaskService.Domain.Entities;

namespace DD.TaskService.Domain.Specifications;

public interface IPlannedRecurrenceSpecification : IEntitySpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>,
    IUserOwnedSpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>;

public class PlannedRecurrenceSpecification : UserOwnedSpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>, IPlannedRecurrenceSpecification;
