namespace DD.Tests.Integration.Helpers;

internal static class AuthTestData
{
    internal static string CreateUniqueUsername(string prefix = "test")
    {
        return $"{prefix}-{Guid.NewGuid():N}";
    }
}
