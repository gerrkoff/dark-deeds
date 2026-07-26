using DD.Shared.TaskText;

namespace DD.ServiceTask.Domain.Services;

// Adapts the service-domain IDateService to the shared parser's local-date abstraction.
internal sealed class TaskTextDateProvider(IDateService dateService) : ITaskTextDateProvider
{
    public DateOnly Today => DateOnly.FromDateTime(dateService.Today);
}
