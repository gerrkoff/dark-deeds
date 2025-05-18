namespace DarkDeeds.Mcp.Helpers;

public static class EnvHelper
{
    public static string GetUserId() => Get("DD_MCP_USER_ID");

    public static string GetApiKey() => Get("DD_MCP_API_KEY");

    public static string GetApiUrl() => Environment.GetEnvironmentVariable("DD_MCP_API_URL") ?? "https://dark-deeds.com/api/mcp";

    private static string Get(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrEmpty(value))
            throw new Exception($"{key} is not set in environment variables");

        return value;
    }
}
