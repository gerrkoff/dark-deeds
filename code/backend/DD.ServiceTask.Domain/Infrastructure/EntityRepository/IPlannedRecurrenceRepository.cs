using DD.ServiceTask.Domain.Entities;

namespace DD.ServiceTask.Domain.Infrastructure.EntityRepository;

public interface IPlannedRecurrenceRepository : IRepository<PlannedRecurrenceEntity>
{
    /// <summary>
    /// Atomically adds a recurrence to PlannedRecurrence if it doesn't already exist for the given date.
    /// </summary>
    /// <param name="plannedRecurrenceUid">UID of the PlannedRecurrence entity.</param>
    /// <param name="recurrence">Recurrence to add.</param>
    /// <returns>True if the recurrence was added, false if it already existed.</returns>
    Task<bool> TryAddRecurrenceAsync(string plannedRecurrenceUid, RecurrenceEntity recurrence);

    /// <summary>
    /// Updates the TaskUid for an existing recurrence identified by date.
    /// </summary>
    /// <param name="plannedRecurrenceUid">UID of the PlannedRecurrence entity.</param>
    /// <param name="dateTime">Date of the recurrence to update.</param>
    /// <param name="taskUid">New TaskUid value.</param>
    /// <returns>True if the update was successful.</returns>
    Task<bool> TryUpdateRecurrenceTaskUidAsync(string plannedRecurrenceUid, DateTime dateTime, string taskUid);
}
