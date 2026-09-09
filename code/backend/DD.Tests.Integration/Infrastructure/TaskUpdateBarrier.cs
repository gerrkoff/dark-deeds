namespace DD.Tests.Integration.Infrastructure;

internal sealed class TaskUpdateBarrier
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    private readonly object _sync = new();
    private string? _uid;
    private int _remainingParticipants;
    private TaskCompletionSource<bool>? _release;

    public void Arm(string uid, int participantCount)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uid);
        ArgumentOutOfRangeException.ThrowIfLessThan(participantCount, 2);

        lock (_sync)
        {
            if (_release is not null)
                throw new InvalidOperationException("A task update barrier is already active.");

            _uid = uid;
            _remainingParticipants = participantCount;
            _release = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }

    public Task WaitAsync(string uid)
    {
        Task waitTask;
        lock (_sync)
        {
            if (_release is null || !string.Equals(_uid, uid, StringComparison.Ordinal))
                return Task.CompletedTask;

            waitTask = _release.Task;
            _remainingParticipants--;
            if (_remainingParticipants == 0)
            {
                _uid = null;
                _release.SetResult(true);
                _release = null;
            }
        }

        return waitTask.WaitAsync(Timeout);
    }
}
