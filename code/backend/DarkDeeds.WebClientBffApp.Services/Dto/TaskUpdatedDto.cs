using System.Collections.Generic;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.WebClientBffApp.Services.Dto
{
    public class TaskUpdatedDto
    {
        public ICollection<TaskDto> Tasks { get; set; }
        public string UserId { get; set; }
    }
}