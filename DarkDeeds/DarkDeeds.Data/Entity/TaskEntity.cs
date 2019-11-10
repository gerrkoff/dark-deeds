using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DarkDeeds.Data.Entity.Base;
using DarkDeeds.Enums;

namespace DarkDeeds.Data.Entity
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