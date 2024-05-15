using DD.MobileClient.Domain.Dto;
using DD.MobileClient.Domain.Entities;
using DD.MobileClient.Domain.Infrastructure;
using DD.MobileClient.Domain.Models;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.Extensions.Logging;

namespace DD.MobileClient.Domain.Services;

public interface IWatchService
{
    Task<WatchWidgetStatusDto> GetWidgetStatus(string mobileKey);

    Task<WatchAppStatusDto> GetAppStatus(string mobileKey);
}

internal sealed class WatchService(
    IMobileUserRepository mobileUserRepository,
    ICacheProvider cacheProvider,
    ITaskServiceApp taskServiceApp,
    ILogger<WatchService> logger) : IWatchService
{
    public async Task<WatchWidgetStatusDto> GetWidgetStatus(string mobileKey)
    {
        var cacheKey = new WidgetStatusCacheKey(mobileKey);

        if (cacheProvider.GetValue<WatchWidgetStatusDto>(cacheKey) is { } cached)
            return cached;

        Log.MissedWidgetStatusCache(logger, mobileKey);

        var user = await GetUser(mobileKey);

        try
        {
            var tasks = await GetTasks(user);

            var header = GetHeader(tasks);

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

            var result = new WatchWidgetStatusDto(header, firstNotCompletedUi, firstNotCompletedIncludingRoutineUi);

            cacheProvider.SetValue(cacheKey, result);

            return result;
        }
#pragma warning disable CA1031
        catch (Exception)
#pragma warning restore CA1031
        {
            return new WatchWidgetStatusDto("🤯 error", string.Empty, string.Empty);
        }
    }

    public async Task<WatchAppStatusDto> GetAppStatus(string mobileKey)
    {
        var cacheKey = new AppStatusCacheKey(mobileKey);

        if (cacheProvider.GetValue<WatchAppStatusDto>(cacheKey) is { } cached)
            return cached;

        Log.MissedAppStatusCache(logger, mobileKey);

        var user = await GetUser(mobileKey);

        try
        {
            var tasks = await GetTasks(user);

            var header = GetHeader(tasks);

            List<WatchAppStatusItemDto> items = [];

            foreach (var task in tasks)
            {
                var item = await taskServiceApp.PrintTasks([task]);
                items.Add(new WatchAppStatusItemDto(item.First(), task.Type == TaskType.Routine));
            }

            var result = new WatchAppStatusDto(header, items);

            cacheProvider.SetValue(cacheKey, result);

            return result;
        }
#pragma warning disable CA1031
        catch (Exception)
#pragma warning restore CA1031
        {
            return new WatchAppStatusDto("🤯 error", []);
        }
    }

    private async Task<MobileUserEntity> GetUser(string mobileKey)
    {
        var user = await mobileUserRepository.GetByMobileKeyAsync(mobileKey)
                   ?? throw new InvalidOperationException($"User with mobile key {mobileKey} not found");
        return user;
    }

    private async Task<List<TaskDto>> GetTasks(MobileUserEntity user)
    {
        var from = DateTime.UtcNow.Date;
        var till = from.AddDays(1);
        var tasks = (await taskServiceApp.LoadTasksByDateAsync(from, till, user.UserId)).OrderBy(x => x.Order)
            .ToList();
        return tasks;
    }

    private static string GetHeader(List<TaskDto> tasks)
    {
        var remaining = tasks.Count(task => task is { Completed: false, Type: TaskType.Simple });
        var header = remaining == 0
            ? "🎉 all finished!"
            : $"\ud83d\udccc {remaining} remaining";

        return header;
    }
}
