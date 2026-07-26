using DD.TerminalClient.Domain.Time;

namespace DD.TerminalClient.Details.Time;

// Derives the local calendar date from the injected TimeProvider's UTC clock and the machine time
// zone. Uses TimeProvider.GetUtcNow() (never the banned DateTime.Now / DateTimeOffset.Now) so a
// client/SSH time-zone change is reflected and the clock stays substitutable in tests.
public sealed class SystemLocalDateProvider(TimeProvider timeProvider) : ILocalDateProvider
{
    public DateOnly Today
    {
        get
        {
            // ConvertTime yields a DateTimeOffset carrying the local offset, so its Year/Month/Day
            // components are the local wall-clock date. Read them directly to avoid the banned
            // DateTimeOffset.DateTime / DateTime.Now family.
            var localNow = TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), TimeZoneInfo.Local);
            return new DateOnly(localNow.Year, localNow.Month, localNow.Day);
        }
    }
}
