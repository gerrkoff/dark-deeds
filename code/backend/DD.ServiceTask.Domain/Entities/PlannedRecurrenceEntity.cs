using DD.ServiceTask.Domain.Entities.Abstractions;
using DD.ServiceTask.Domain.Entities.Enums;

namespace DD.ServiceTask.Domain.Entities;

public class PlannedRecurrenceEntity : Entity, IUserOwnedEntity
{
    public string Task { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? EveryNthDay { get; set; }
    public string EveryMonthDay { get; set; }
    public RecurrenceWeekdayEnum? EveryWeekday { get; set; }
    public string UserId { get; set; }
    public List<RecurrenceEntity> Recurrences { get; set; } = new List<RecurrenceEntity>();
}
