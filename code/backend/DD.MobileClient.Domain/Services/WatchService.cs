using DD.MobileClient.Domain.Dto;
using DD.MobileClient.Domain.Infrastructure;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;

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
            var from = DateTime.UtcNow.Date;
            var till = from.AddDays(1);
            var tasks = (await taskServiceApp.LoadTasksByDateAsync(from, till, user.UserId)).OrderBy(x => x.Order)
                .ToList();

            var remaining = tasks.Count(task => task is { Completed: false, Type: TaskType.Simple });
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

            var header = remaining == 0
                ? "🎉 all finished!"
                : $"{remaining} remaining";

            return new WatchStatusDto(header, firstNotCompletedUi, firstNotCompletedIncludingRoutineUi);
        }
#pragma warning disable CA1031
        catch (Exception)
#pragma warning restore CA1031
        {
            return new WatchStatusDto(string.Empty, "🤯 error", string.Empty);
        }
    }
}
