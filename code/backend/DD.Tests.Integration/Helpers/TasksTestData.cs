namespace DD.Tests.Integration.Helpers;

internal static class TasksTestData
{
    internal static string CreateUniqueTaskUid()
    {
        return Guid.NewGuid().ToString();
    }
}
