using System.Collections.Generic;
using DarkDeeds.ServiceTask.Models.Dto;

namespace DarkDeeds.ServiceTask.Infrastructure.Services.Dto
{
    public class TaskUpdatedDto
    {
        public ICollection<TaskDto> Tasks { get; set; }
        public string UserId { get; set; }
    }
}