using DD.Tests.Integration.Infrastructure.ExternalDependencies;

namespace DD.Tests.Integration.Infrastructure;

internal sealed class IntegrationExternalDependencies
{
    internal TestBotSendMessageService BotMessages { get; } = new();
}
