using DD.Shared.Data.Abstractions;

namespace DD.TelegramClient.Domain.Entities;

public class TelegramUserEntity : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public string TelegramChatKey { get; set; } = string.Empty;

    public int TelegramChatId { get; set; }

    public int TelegramTimeAdjustment { get; set; }
}
