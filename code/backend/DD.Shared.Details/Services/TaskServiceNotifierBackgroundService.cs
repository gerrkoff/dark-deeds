using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DD.Shared.Details.Services;

public class TaskServiceNotifierBackgroundService(
    IServiceProvider serviceProvider,
    ITaskServiceNotifierChannelProvider channelProvider,
    ILogger<TaskServiceNotifierBackgroundService> logger) : IHostedService
{
    private Task? _backgroundTask;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _backgroundTask = Task.Run(
            async () =>
            {
                await foreach (var updatedTasksDto in channelProvider.ReadAllAsync(cancellationToken))
                {
                    await TaskUpdated(updatedTasksDto);
                }
            },
            cancellationToken);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        channelProvider.Complete();
        if (_backgroundTask != null)
            await _backgroundTask;
    }

    public async Task AddTaskUpdated(TasksUpdatedDto updatedTasks)
    {
        await channelProvider.WriteAsync(updatedTasks);
    }

    private async Task TaskUpdated(TasksUpdatedDto updatedTasks)
    {
        using var scope = serviceProvider.CreateScope();
        var subscribers = scope.ServiceProvider.GetServices<ITaskServiceSubscriber>();

        foreach (var subscriber in subscribers)
        {
            try
            {
                await subscriber.TasksUpdated(updatedTasks);
            }
#pragma warning disable CA1031
            catch (Exception ex)
#pragma warning restore CA1031
            {
                Log.FailedToNotifyAboutTaskUpdated(logger, ex.Message);
            }
        }
    }
}
