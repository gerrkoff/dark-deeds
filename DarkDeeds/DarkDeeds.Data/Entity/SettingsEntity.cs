using System.ComponentModel.DataAnnotations;
using DarkDeeds.Data.Entity.Base;

namespace DarkDeeds.Data.Entity
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