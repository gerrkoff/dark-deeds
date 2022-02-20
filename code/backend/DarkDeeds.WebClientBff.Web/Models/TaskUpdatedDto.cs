using System.Collections.Generic;
using DarkDeeds.ServiceTask.Dto;

namespace DarkDeeds.WebClientBff.Web.Models
{
    public class TaskUpdatedDto
    {
        public ICollection<TaskDto> Tasks { get; set; }
        public string UserId { get; set; }
    }
}
