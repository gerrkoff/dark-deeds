using System.Collections.Generic;
using DD.TaskService.Domain.Dto;

namespace DarkDeeds.WebClientBff.Web.Models
{
    public class TaskUpdatedDto
    {
        public ICollection<TaskDto> Tasks { get; set; }
        public string UserId { get; set; }
    }
}
