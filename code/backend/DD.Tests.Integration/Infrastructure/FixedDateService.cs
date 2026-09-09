using ServiceTaskDateService = DD.ServiceTask.Domain.Services.IDateService;
using TelegramDateService = DD.TelegramClient.Domain.Services.IDateService;

namespace DD.Tests.Integration.Infrastructure;

internal sealed class FixedDateService : ServiceTaskDateService, TelegramDateService
{
    public DateTime Today => IntegrationTestClock.UtcToday;

    public DateTime Now => IntegrationTestClock.UtcNow;
}
