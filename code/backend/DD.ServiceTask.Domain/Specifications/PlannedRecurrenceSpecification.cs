using DD.ServiceTask.Domain.Entities;

namespace DD.ServiceTask.Domain.Specifications;

public interface IPlannedRecurrenceSpecification : IEntitySpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>,
    IUserOwnedSpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>;

public class PlannedRecurrenceSpecification : UserOwnedSpecification<PlannedRecurrenceEntity, IPlannedRecurrenceSpecification>, IPlannedRecurrenceSpecification;
