using System;
using DarkDeeds.Enums;

namespace DarkDeeds.Models
{
    public class TaskDto
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
        public TaskTimeTypeEnum TimeType { get; set; }
        public int Version { get; set; }
    }
}