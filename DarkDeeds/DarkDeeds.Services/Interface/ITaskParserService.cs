using System.Collections.Generic;
using DarkDeeds.Models;

namespace DarkDeeds.Services.Interface
{
    public interface ITaskParserService
    {
        TaskDto ParseTask(string task);
        // TODO: remove time adjustment
        string PrintTasks(IEnumerable<TaskDto> tasks, int timeAdjustment = 0);
    }
}