namespace DD.Tests.Integration.Infrastructure;

internal static class IntegrationCleanup
{
    internal static void ReportFailure(Exception exception)
    {
        Console.Error.WriteLine($"Integration test cleanup failed: {exception}");
    }
}
