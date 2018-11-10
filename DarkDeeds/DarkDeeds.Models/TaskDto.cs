using System;

namespace DarkDeeds.Models
{
    public class TaskDto
    {
        public int Id { get; set; }
        public DateTime? DateTime { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public int ClientId { get; set; }
        public bool Completed { get; set; }
    }
}