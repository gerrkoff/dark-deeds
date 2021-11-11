using System;

namespace DarkDeeds.LoadTests
{
    public static class Env
    {
        static Env()
        {
            Console.WriteLine($"DOMAIN={Domain}\nTEST_TIME={TestTime}\nTEST1_RPS={Test1Rps}\nTEST2_RPS={Test2Rps}\nTEST3_RPS={Test3Rps}");
        }
        
        public static readonly string Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? "test.dark-deeds.com";

        public static readonly int TestTime =
            int.TryParse(Environment.GetEnvironmentVariable("TEST_TIME"), out int v) ? v : 60 * 5;
        
        public static readonly int Test1Rps =
            int.TryParse(Environment.GetEnvironmentVariable("TEST1_RPS"), out int v) ? v : 50;
        public static readonly int Test2Rps =
            int.TryParse(Environment.GetEnvironmentVariable("TEST2_RPS"), out int v) ? v : 20;
        public static readonly int Test3Rps =
            int.TryParse(Environment.GetEnvironmentVariable("TEST3_RPS"), out int v) ? v : 25;
    }
}