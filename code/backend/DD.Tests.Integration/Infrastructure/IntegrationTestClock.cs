using ServiceTaskDateService = DD.ServiceTask.Domain.Services.IDateService;
using TelegramDateService = DD.TelegramClient.Domain.Services.IDateService;

namespace DD.Tests.Integration.Infrastructure;

internal sealed class IntegrationTestClock : ServiceTaskDateService, TelegramDateService
{
    public static DateTime UtcNow { get; } = DateTime.UtcNow;

    public static DateTime UtcToday => UtcNow.Date;

    public DateTime Today => UtcToday;

    public DateTime Now => UtcNow;
}
