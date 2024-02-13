using System.Text.Json;

namespace DarkDeeds.E2eTests.Models;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions I = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
