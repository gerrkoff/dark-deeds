using System;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Models.Dto.Base;

namespace DarkDeeds.TaskServiceApp.Models.Dto
{
    public class TaskDto : IDtoWithId
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int? Time { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public int ClientId { get; set; }
        public bool Completed { get; set; }
        public bool IsProbable { get; set; }
        public bool Deleted { get; set; }
        public TaskTypeEnum Type { get; set; }
        public int Version { get; set; }
    }
}