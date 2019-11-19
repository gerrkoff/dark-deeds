using System.Collections.Generic;
using DarkDeeds.Models;

namespace DarkDeeds.Services.Interface
{
    public interface ITaskParserService
    {
        TaskDto ParseTask(string task, bool ignoreDate = false);
        string PrintTasks(IEnumerable<TaskDto> tasks);
    }
}