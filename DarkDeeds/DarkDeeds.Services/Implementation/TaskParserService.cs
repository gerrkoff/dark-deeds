using DarkDeeds.Models;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class TaskParserService : ITaskParserService
    {
        // TODO: implement this
        public TaskDto ParseTask(string task)
        {
            return new TaskDto
            {
                Title = task
            };
        }
    }
}