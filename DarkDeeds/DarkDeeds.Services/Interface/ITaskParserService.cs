using DarkDeeds.Models;

namespace DarkDeeds.Services.Interface
{
    public interface ITaskParserService
    {
        TaskDto ParseTask(string task);
    }
}