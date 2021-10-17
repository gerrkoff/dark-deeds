using System.ComponentModel.DataAnnotations;
using DarkDeeds.WebClientBffApp.Entities.Base;

namespace DarkDeeds.WebClientBffApp.Entities
{
    public class SettingsEntity : BaseEntity
    {
        public bool ShowCompleted { get; set; }
        
        [Required]
        public string UserId { get; set; }
    }
}