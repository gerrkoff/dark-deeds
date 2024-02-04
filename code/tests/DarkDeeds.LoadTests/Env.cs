using System;
using Xunit;

namespace DarkDeeds.LoadTests;

public static class Env
{
    static Env()
    {
        Assert.NotEmpty(Domain);
        Console.WriteLine($"DOMAIN={Domain}");
    }

    public static readonly string Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? "test.dark-deeds.com";
}
