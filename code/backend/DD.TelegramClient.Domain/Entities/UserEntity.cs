using System.ComponentModel.DataAnnotations;
using DD.Shared.Data.Abstractions;

namespace DD.TelegramClient.Domain.Entities;

public class TelegramUserEntity : BaseEntity
{
    [Required]
    public string UserId { get; set; }
    public string TelegramChatKey { get; set; }
    public int TelegramChatId { get; set; }
    public int TelegramTimeAdjustment { get; set; }
}
