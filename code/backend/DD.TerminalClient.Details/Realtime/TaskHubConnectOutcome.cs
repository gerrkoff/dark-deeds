namespace DD.TerminalClient.Details.Realtime;

// The result of a single connect attempt, so the reconnect loop and the initial start branch identically:
// a success clears backoff, an unauthorized result stops all retries and surfaces re-login, a failure
// schedules the next backoff step, and a cancelled attempt means the client is stopping.
internal enum TaskHubConnectOutcome
{
    Connected,
    Unauthorized,
    Failed,
    Cancelled,
}
