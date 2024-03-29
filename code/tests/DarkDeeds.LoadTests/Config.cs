using System.Text.Json;

namespace DarkDeeds.LoadTests;

public class Config
{
    private readonly Settings _settings;

    public Config()
    {
        var json = File.ReadAllText("appsettings.json");
        var settings = JsonSerializer.Deserialize<Settings>(json);
        ArgumentNullException.ThrowIfNull(settings);
        _settings = settings;
    }

    public static int Timeout => 60;

    public static int TimeWarmUp => 5;

    public int TimeRamp => _settings.TimeRamp;

    public int TimeTest => _settings.TimeTest;

    public int Test1Rps => _settings.Test1Rps;

    public int Test1MaxLatency => _settings.Test1MaxLatency;

    public int Test2Rps => _settings.Test2Rps;

    public int Test2MaxLatency => _settings.Test2MaxLatency;

    public int Test3Rps => _settings.Test3Rps;

    public int Test3MaxLatency => _settings.Test3MaxLatency;

    public int Test4Rps => _settings.Test4Rps;

    public int Test4MaxLatency => _settings.Test4MaxLatency;

    private sealed class Settings
    {
        public int TimeRamp { get; init; }

        public int TimeTest { get; init; }

        public int Test1Rps { get; init; }

        public int Test1MaxLatency { get; set; }

        public int Test2Rps { get; init; }

        public int Test2MaxLatency { get; set; }

        public int Test3Rps { get; init; }

        public int Test3MaxLatency { get; set; }

        public int Test4Rps { get; init; }

        public int Test4MaxLatency { get; set; }
    }
}
