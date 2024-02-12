namespace DD.TelegramClient.Domain.Implementation;

public interface IDateService
{
    DateTime Today { get; }

    DateTime Now { get; }
}

internal sealed class DateService : IDateService
{
    public DateTime Today => DateTime.UtcNow.Date;

    public DateTime Now => DateTime.UtcNow;
}
