using System;

namespace DarkDeeds.LoadTests
{
    public static class Env
    {
        static Env()
        {
            Console.WriteLine($"DOMAIN={Domain}");
        }
        
        public static readonly string Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? "test.dark-deeds.com";
    }
}