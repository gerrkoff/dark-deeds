namespace DD.TerminalClient.Details.Realtime;

// The reconnect backoff schedule, matching the web client exactly: 1, 2, 4, 8, 16 seconds and then a
// steady 30-second ceiling for every later attempt. Pure and stateless - the caller tracks the attempt
// number - so the sequence is verified in isolation without waiting on a real clock.
internal static class TerminalRetryPolicy
{
    public const int MaxDelaySeconds = 30;

    public static TimeSpan GetDelay(int attempt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(attempt);

        var seconds = attempt >= 5 ? MaxDelaySeconds : 1 << attempt;
        return TimeSpan.FromSeconds(seconds);
    }
}
