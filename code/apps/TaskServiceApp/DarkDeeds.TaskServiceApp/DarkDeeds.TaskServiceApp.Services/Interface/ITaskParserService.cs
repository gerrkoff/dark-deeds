using System.Collections.Generic;
using DarkDeeds.TaskServiceApp.Models.Dto;

namespace DarkDeeds.TaskServiceApp.Services.Interface
{
    public interface ITaskParserService
    {
        TaskDto ParseTask(string task, bool ignoreDate = false);
        IList<string> PrintTasks(IEnumerable<TaskDto> tasks);
    }
}