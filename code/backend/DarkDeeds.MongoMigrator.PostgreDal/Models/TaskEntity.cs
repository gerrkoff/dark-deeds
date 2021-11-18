using System;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.MongoMigrator.PostgreDal.Models.Base;
using DarkDeeds.MongoMigrator.PostgreDal.Models.Enums;

namespace DarkDeeds.MongoMigrator.PostgreDal.Models
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
        
        public string Uid { get; set; }
        
        [Required]
        public string UserId { get; set; }
    }
}