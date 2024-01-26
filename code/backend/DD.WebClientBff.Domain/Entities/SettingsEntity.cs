using System.ComponentModel.DataAnnotations;
using DD.Shared.Data.Abstractions;

namespace DD.WebClientBff.Domain.Entities;

public class SettingsEntity : BaseEntity
{
    [Required]
    public string UserId { get; set; }
    public bool ShowCompleted { get; set; }
}
