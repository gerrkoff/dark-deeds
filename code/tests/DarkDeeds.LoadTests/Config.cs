using System.IO;
using System.Text.Json;

namespace DarkDeeds.LoadTests
{
    public class Config
    {
        private readonly Settings _settings;

        public Config()
        {
            var json = File.ReadAllText("appsettings.json");
            _settings = JsonSerializer.Deserialize<Settings>(json);
        }

        public int Timeout => 60;
        public int TimeWarmUp => 5;
        public int TimeRamp => _settings.TimeRamp;
        public int TimeTest => _settings.TimeTest;
        public int Test1Rps => _settings.Test1Rps;
        public int Test2Rps => _settings.Test2Rps;
        public int Test3Rps => _settings.Test3Rps;
        public int Test4Rps => _settings.Test4Rps;

        class Settings
        {
            public int TimeRamp { get; set; }
            public int TimeTest { get; set; }
            public int Test1Rps { get; set; }
            public int Test2Rps { get; set; }
            public int Test3Rps { get; set; }
            public int Test4Rps { get; set; }
        }
    }
}