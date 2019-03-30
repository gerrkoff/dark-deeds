using System.Collections.Generic;
using DarkDeeds.Models;

namespace DarkDeeds.Services.Interface
{
    public interface ITaskParserService
    {
        TaskDto ParseTask(string task, int timeAdjustment = 0);
        string PrintTasks(IEnumerable<TaskDto> tasks, int timeAdjustment = 0);
    }
}