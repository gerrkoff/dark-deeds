using System.Collections.Generic;
using DarkDeeds.WebClientBff.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.WebClientBff.Services.Dto
{
    public class TaskUpdatedDto
    {
        public ICollection<TaskDto> Tasks { get; set; }
        public string UserId { get; set; }
    }
}