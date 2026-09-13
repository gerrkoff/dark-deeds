namespace DD.Tests.Integration.Helpers;

internal static class TasksHelper
{
    internal static string CreateUniqueTaskUid()
    {
        return Guid.NewGuid().ToString();
    }
}
