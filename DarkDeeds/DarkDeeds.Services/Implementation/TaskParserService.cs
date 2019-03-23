using System.Collections.Generic;
using System.Text;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class TaskParserService : ITaskParserService
    {
        // TODO: implement this
        // TODO: unit-tests
        public TaskDto ParseTask(string task)
        {
            return new TaskDto
            {
                Title = task
            };
        }

        public string PrintTasks(IEnumerable<TaskDto> tasks)
        {
            var sb = new StringBuilder();
            foreach (var task in tasks)
            {
                if (sb.Length > 0)
                    sb.AppendLine();
                sb.Append(TaskToString(task));
            }

            return sb.ToString();
        }

        private string TaskToString(TaskDto task) => $"{task.Title}";
    }
}