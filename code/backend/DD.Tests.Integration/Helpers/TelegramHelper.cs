namespace DD.Tests.Integration.Helpers;

internal static class TelegramHelper
{
    private static int _nextChatId = -100000;

    public static int CreateUniqueChatId()
    {
        return Interlocked.Decrement(ref _nextChatId);
    }
}
