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

public sealed class WatchService(
    IMobileUserRepository mobileUserRepository,
    ICacheProvider cacheProvider,
    ITaskServiceApp taskServiceApp,
    IWatchPayloadController watchPayloadController,
    ILogger<WatchService> logger) : IWatchService
{
    public async Task<WatchWidgetStatusDto> GetWidgetStatus(string mobileKey)
    {
        var from = DateTime.UtcNow.Date;
        var cacheKey = new WidgetStatusCacheKey(mobileKey);

        if (cacheProvider.GetValue<StatusCacheItem<WatchWidgetStatusDto>>(cacheKey) is { } cached
            && cached.Date == from)
        {
            Log.HitWidgetStatusCache(logger, mobileKey);
            return cached.Item;
        }

        Log.MissedWidgetStatusCache(logger, mobileKey);

        var user = await GetUser(mobileKey);

        try
        {
            var tasks = await GetTasks(user, from);

            var payload = watchPayloadController.GetWidgetStatus(tasks);

            cacheProvider.SetValue(cacheKey, payload);

            return payload;
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
        var from = DateTime.UtcNow.Date;
        var cacheKey = new AppStatusCacheKey(mobileKey);

        if (cacheProvider.GetValue<StatusCacheItem<WatchAppStatusDto>>(cacheKey) is { } cached
            && cached.Date == from)
        {
            Log.HitAppStatusCache(logger, mobileKey);
            return cached.Item;
        }

        Log.MissedAppStatusCache(logger, mobileKey);

        var user = await GetUser(mobileKey);

        try
        {
            var tasks = await GetTasks(user, from);

            var payload = watchPayloadController.GetAppStatus(tasks);

            cacheProvider.SetValue(cacheKey, payload);

            return payload;
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

    private async Task<List<TaskDto>> GetTasks(MobileUserEntity user, DateTime from)
    {
        var till = from.AddDays(1);
        return (await taskServiceApp.LoadTasksByDateAsync(from, till, user.UserId)).ToList();
    }
}
