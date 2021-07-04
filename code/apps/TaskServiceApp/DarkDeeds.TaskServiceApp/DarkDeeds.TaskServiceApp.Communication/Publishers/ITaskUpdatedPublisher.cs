using System.Collections.Generic;
using DarkDeeds.TaskServiceApp.Models.Dto;

namespace DarkDeeds.TaskServiceApp.Communication.Publishers
{
    public interface ITaskUpdatedPublisher
    {
        void Send(ICollection<TaskDto> tasks);
    }
}