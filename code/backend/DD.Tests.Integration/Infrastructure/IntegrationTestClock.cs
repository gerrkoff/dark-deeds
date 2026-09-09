namespace DD.Tests.Integration.Infrastructure;

internal static class IntegrationTestClock
{
    public static DateTime UtcNow { get; } = DateTime.UtcNow;

    public static DateTime UtcToday => UtcNow.Date;
}
