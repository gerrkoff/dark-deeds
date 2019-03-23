using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Models;

namespace DarkDeeds.BotIntegration.Interface.CommandProcessor
{
    public interface ICreateTaskCommandProcessor
    {
        Task ProcessAsync(CreateTaskCommand command);
        void BindSendUpdateTasks(Action<IEnumerable<TaskDto>> sendUpdateTasks);
    }
}