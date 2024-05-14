using DD.MobileClient.Domain.Infrastructure;
using DD.MobileClient.Domain.Models;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.MobileClient.Domain.Subscriptions;

public class TaskServiceSubscriber(
    IMobileUserRepository mobileUserRepository,
    ICacheProvider cacheProvider) : ITaskServiceSubscriber
{
    public async Task TasksUpdated(TasksUpdatedDto tasksUpdated)
    {
        var user = await mobileUserRepository.GetByIdAsync(tasksUpdated.UserId);

        if (user is null)
            return;

        cacheProvider.Evict(new AppStatusCacheKey(user.MobileKey));
        cacheProvider.Evict(new WidgetStatusCacheKey(user.MobileKey));
    }
}
