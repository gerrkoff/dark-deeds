using System.Diagnostics.CodeAnalysis;
using System.Runtime.Loader;

namespace DD.Tests.Integration.Infrastructure;

internal static class IntegrationEnvironmentLifetime
{
    private static readonly IntegrationExternalDependencies ExternalDependencies = new();
    private static readonly Lazy<Task<IntegrationEnvironment>> Shared =
        new(
            () => IntegrationEnvironment.CreateAsync(ExternalDependencies),
            LazyThreadSafetyMode.ExecutionAndPublication);

    private static int _cleanupStarted;

    static IntegrationEnvironmentLifetime()
    {
        AssemblyLoadContext.Default.Unloading += _ => CleanupAtProcessExit();
        AppDomain.CurrentDomain.ProcessExit += (_, _) => CleanupAtProcessExit();
    }

    internal static async Task<HttpClient> CreateClientAsync(bool allowAutoRedirect = true)
    {
        var environment = await Shared.Value;
        return environment.CreateClient(allowAutoRedirect);
    }

    internal static async Task<HttpMessageHandler> CreateSignalRHandlerAsync()
    {
        var environment = await Shared.Value;
        return environment.CreateSignalRHandler();
    }

    internal static async Task<RecordingBotSendMessageService> GetTelegramMessagesAsync()
    {
        _ = await Shared.Value;
        return ExternalDependencies.TelegramMessages;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Process teardown must report cleanup failures without escaping the process-exit callback.")]
    private static void CleanupAtProcessExit()
    {
        if (!Shared.IsValueCreated || Interlocked.Exchange(ref _cleanupStarted, 1) != 0)
            return;

        try
        {
            var environment = Shared.Value
                .WaitAsync(IntegrationEnvironment.CleanupTimeout)
                .GetAwaiter()
                .GetResult();
            environment.DisposeAsync()
                .AsTask()
                .WaitAsync(IntegrationEnvironment.CleanupTimeout)
                .GetAwaiter()
                .GetResult();
        }
        catch (Exception exception)
        {
            IntegrationCleanup.ReportFailure(exception);
        }
    }
}
