namespace DD.Tests.Integration.Helpers;

internal static class Helper
{
    public static string CreateUniqueUsername(string prefix = "test")
    {
        return $"{prefix}-{Guid.NewGuid():N}";
    }

    public static string CreateUniqueTaskUid()
    {
        return Guid.NewGuid().ToString();
    }

    public static void ReportCleanupFailure(Exception exception)
    {
        Console.Error.WriteLine($"Integration test cleanup failed: {exception}");
    }
}
