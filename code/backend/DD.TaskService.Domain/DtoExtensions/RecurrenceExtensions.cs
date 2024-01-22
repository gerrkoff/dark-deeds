using DD.TaskService.Domain.Dto;

namespace DD.TaskService.Domain.DtoExtensions;

static class RecurrenceExtensions
{
    public static ICollection<PlannedRecurrenceDto> ToUtcDate(this ICollection<PlannedRecurrenceDto> recurrences)
    {
        foreach (var recurrence in recurrences)
        {
            recurrence.StartDate = DateTime.SpecifyKind(recurrence.StartDate, DateTimeKind.Utc);
            if (recurrence.EndDate != null)
                recurrence.EndDate = DateTime.SpecifyKind(recurrence.EndDate.Value, DateTimeKind.Utc);
        }

        return recurrences;
    }
}
