using System.ComponentModel.DataAnnotations;

namespace DarkDeeds.Backend.Entities.Entity
{
    public class SettingsEntity : BaseEntity
    {
        public bool ShowCompleted { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        public virtual UserEntity User { get; set; }
    }
}