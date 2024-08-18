using System.Threading.Channels;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.Shared.Details.Common;

public interface ITaskServiceNotifierChannelProvider
{
    Task WriteAsync(TasksUpdatedDto updatedTasks);

    IAsyncEnumerable<TasksUpdatedDto> ReadAllAsync(CancellationToken cancellationToken);

    void Complete();
}

public class TaskServiceNotifierChannelProvider : ITaskServiceNotifierChannelProvider
{
    private readonly Channel<TasksUpdatedDto> _channel = Channel.CreateUnbounded<TasksUpdatedDto>();

    public async Task WriteAsync(TasksUpdatedDto updatedTasks)
    {
        await _channel.Writer.WriteAsync(updatedTasks);
    }

    public IAsyncEnumerable<TasksUpdatedDto> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }

    public void Complete()
    {
        _channel.Writer.Complete();
    }
}
