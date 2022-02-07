using System.ComponentModel.DataAnnotations;
using DarkDeeds.WebClientBff.Entities.Base;

namespace DarkDeeds.WebClientBff.Entities
{
    public class SettingsEntity : BaseEntity
    {
        public bool ShowCompleted { get; set; }
        
        [Required]
        public string UserId { get; set; }
    }
}