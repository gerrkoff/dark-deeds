using DD.MobileClient.Domain.Dto;
using DD.MobileClient.Domain.Infrastructure;
using DD.MobileClient.Domain.Infrastructure.Dto;

namespace DD.MobileClient.Domain.Services;

public interface IWatchService
{
    Task<WatchStatusDto> GetStatus(string mobileKey);
}

internal sealed class WatchService(
    IMobileUserRepository mobileUserRepository,
    ITaskServiceApp taskServiceApp) : IWatchService
{
    public async Task<WatchStatusDto> GetStatus(string mobileKey)
    {
        var user = await mobileUserRepository.GetByMobileKeyAsync(mobileKey)
                     ?? throw new InvalidOperationException($"User with mobile key {mobileKey} not found");

        try
        {
            var from = DateTime.Today;
            var till = from.AddHours(25);
            var tasks = (await taskServiceApp.LoadTasksByDateAsync(from, till, user.UserId)).OrderBy(x => x.Order)
                .ToList();

            var remaining = tasks.Count(task => task is { Completed: false, Type: TaskType.Simple });
            var remainingIncludingRoutine = tasks.Count(task => task is { Completed: false, Type: TaskType.Simple or TaskType.Routine });
            var firstNotCompleted = tasks.FirstOrDefault(task => task is { Completed: false, Type: TaskType.Simple });
            var firstNotCompletedIncludingRoutine = tasks.FirstOrDefault(task =>
                task is { Completed: false, Type: TaskType.Simple or TaskType.Routine });

            var firstNotCompletedUi = firstNotCompleted != null
                ? (await taskServiceApp.PrintTasks([firstNotCompleted])).First()
                : string.Empty;

            var firstNotCompletedIncludingRoutineUi = firstNotCompletedIncludingRoutine != null &&
                                                      firstNotCompletedIncludingRoutine.Type == TaskType.Routine
                ? (await taskServiceApp.PrintTasks([firstNotCompletedIncludingRoutine])).First()
                : string.Empty;

            firstNotCompletedIncludingRoutineUi = firstNotCompletedIncludingRoutineUi.Length > 2
                ? firstNotCompletedIncludingRoutineUi[..^2]
                : firstNotCompletedIncludingRoutineUi;

            var remainingIncludingRoutineSuffix = remainingIncludingRoutine == remaining
                ? string.Empty
                : $" ({remainingIncludingRoutine})";
            var header = remainingIncludingRoutine == 0
                ? "🎉 all finished!"
                : $"{remaining}{remainingIncludingRoutineSuffix} remaining";

            return new WatchStatusDto(header, firstNotCompletedUi, firstNotCompletedIncludingRoutineUi);
        }
#pragma warning disable CA1031
        catch (Exception e)
#pragma warning restore CA1031
        {
            return new WatchStatusDto(string.Empty, "🤯 error", string.Empty);
        }
    }
}
