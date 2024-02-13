using Xunit;

namespace DarkDeeds.LoadTests;

public static class Env
{
    public static readonly string Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? "test.dark-deeds.com";

    static Env()
    {
        Assert.NotEmpty(Domain);
        Console.WriteLine($"DOMAIN={Domain}");
    }
}
