using System;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.Entities.Enums;
using DarkDeeds.Entities.Models.Base;

namespace DarkDeeds.Entities.Models
{
    public class TaskEntity : DeletableEntity, IUserOwnedEntity
    {
        public string Title { get; set; }
        public int Order { get; set; }
        
        public DateTime? Date { get; set; }
        public TaskTypeEnum Type { get; set; }
        public int? Time { get; set; }
        
        public bool IsCompleted { get; set; }
        public bool IsProbable { get; set; }

        public int Version { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public UserEntity User { get; set; }
    }
}