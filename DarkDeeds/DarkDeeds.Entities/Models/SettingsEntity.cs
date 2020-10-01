using System.ComponentModel.DataAnnotations;
using DarkDeeds.Entities.Models.Base;

namespace DarkDeeds.Entities.Models
{
    public class SettingsEntity : BaseEntity
    {
        public bool ShowCompleted { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public UserEntity User { get; set; }
    }
}