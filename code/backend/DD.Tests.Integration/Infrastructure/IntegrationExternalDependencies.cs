namespace DD.Tests.Integration.Infrastructure;

internal sealed class IntegrationExternalDependencies
{
    internal RecordingBotSendMessageService TelegramMessages { get; } = new();
}
