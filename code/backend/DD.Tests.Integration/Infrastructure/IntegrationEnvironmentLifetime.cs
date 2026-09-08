using System.Diagnostics.CodeAnalysis;
using System.Runtime.Loader;
using static DD.Tests.Integration.Helpers.Helper;

namespace DD.Tests.Integration.Infrastructure;

internal static class IntegrationEnvironmentLifetime
{
    private static readonly Lazy<Task<IntegrationEnvironment>> Shared =
        new(IntegrationEnvironment.CreateAsync, LazyThreadSafetyMode.ExecutionAndPublication);

    private static int _cleanupStarted;

    static IntegrationEnvironmentLifetime()
    {
        AssemblyLoadContext.Default.Unloading += _ => CleanupAtProcessExit();
        AppDomain.CurrentDomain.ProcessExit += (_, _) => CleanupAtProcessExit();
    }

    internal static async Task<HttpClient> CreateClientAsync()
    {
        var environment = await Shared.Value;
        return environment.CreateClient();
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
            ReportCleanupFailure(exception);
        }
    }
}
