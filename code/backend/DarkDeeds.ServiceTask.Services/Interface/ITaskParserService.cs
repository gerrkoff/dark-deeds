using System.Collections.Generic;
using DarkDeeds.ServiceTask.Models.Dto;

namespace DarkDeeds.ServiceTask.Services.Interface
{
    public interface ITaskParserService
    {
        TaskDto ParseTask(string task, bool ignoreDate = false);
        // TODO: extract to telegram client
        IList<string> PrintTasks(IEnumerable<TaskDto> tasks);
    }
}