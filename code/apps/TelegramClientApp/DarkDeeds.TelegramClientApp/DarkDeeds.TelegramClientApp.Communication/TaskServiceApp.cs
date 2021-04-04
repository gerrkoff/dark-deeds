using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp.Dto;
using Microsoft.Extensions.Options;

namespace DarkDeeds.TelegramClientApp.Communication
{
    public class TaskServiceApp : ServiceAppBase, ITaskServiceApp
    {
        private readonly string _url;

        public TaskServiceApp(IOptions<CommunicationSettings> communicationSettings)
        {
            _url = $"http://{communicationSettings.Value.TaskService}/api";
        }

        public async Task<IEnumerable<TaskDto>> LoadActualTasksAsync(string userId, DateTime from)
        {
            var url = $"{_url}/tasks?from={DateToString(from)}&userId={userId}";
            var response = await HttpClient.GetAsync(url);
            return await ParseBodyAsync<IEnumerable<TaskDto>>(response);
        }

        public async Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to)
        {
            var url = $"{_url}/tasks/byDate?from={DateToString(from)}&to={DateToString(to)}&userId={userId}";
            var response = await HttpClient.GetAsync(url);
            return await ParseBodyAsync<IEnumerable<TaskDto>>(response);
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId)
        {
            var url = $"{_url}/tasks?userId={userId}";
            var response = await HttpClient.PostAsync(url, SerializePayload(tasks));
            return await ParseBodyAsync<IEnumerable<TaskDto>>(response);
        }

        public async Task<int> CreateRecurrencesAsync(int timezoneOffset, string userId)
        {
            var url = $"{_url}/recurrences/create?timezoneOffset={timezoneOffset}&userId={userId}";
            var response = await HttpClient.PostAsync(url, null);
            return await ParseBodyAsync<int>(response);
        }

        public async Task<IEnumerable<PlannedRecurrenceDto>> LoadRecurrencesAsync(string userId)
        {
            var url = $"{_url}/recurrences?userId={userId}";
            var response = await HttpClient.GetAsync(url);
            return await ParseBodyAsync<IEnumerable<PlannedRecurrenceDto>>(response);
        }

        public async Task<int> SaveRecurrencesAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId)
        {
            var url = $"{_url}/recurrences?userId={userId}";
            var response = await HttpClient.PostAsync(url, SerializePayload(recurrences));
            return await ParseBodyAsync<int>(response);
        }

        public async Task<TaskDto> ParseTask(string text)
        {
            var url = $"{_url}/parser?text={text}";
            var response = await HttpClient.GetAsync(url);
            return await ParseBodyAsync<TaskDto>(response);
        }

        public async Task<string> PrintTasks(IEnumerable<TaskDto> tasks)
        {
            var url = $"{_url}/parser/print";
            var response = await HttpClient.PostAsync(url, SerializePayload(tasks));
            return await ParseBodyAsync<string>(response);
        }
    }
}