using DD.TerminalClient.Details.Time;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Confirms the Details adapter derives the local date from the injected TimeProvider (never a wall
// clock) and applies the machine time zone.
public class SystemLocalDateProviderTests
{
    [Fact]
    public void Today_DerivesLocalDateFromInjectedClock()
    {
        var utcNow = new DateTimeOffset(2000, 1, 15, 10, 0, 0, TimeSpan.Zero);
        var provider = new SystemLocalDateProvider(new StubTimeProvider(utcNow));

        var localNow = TimeZoneInfo.ConvertTime(utcNow, TimeZoneInfo.Local);
        var expected = new DateOnly(localNow.Year, localNow.Month, localNow.Day);

        Assert.Equal(expected, provider.Today);
    }

    [Fact]
    public void Today_ReflectsClockAdvancing()
    {
        var stub = new StubTimeProvider(new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero));
        var provider = new SystemLocalDateProvider(stub);
        var first = provider.Today;

        stub.UtcNow = new DateTimeOffset(2026, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var second = provider.Today;

        Assert.NotEqual(first, second);
    }

    private sealed class StubTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public DateTimeOffset UtcNow { get; set; } = utcNow;

        public override DateTimeOffset GetUtcNow()
        {
            return UtcNow;
        }
    }
}
