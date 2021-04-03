using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.WebClientBffApp.Communication
{
    public class TaskServiceApp : ServiceAppBase, ITaskServiceApp
    {
        private const string Url = "http://localhost:5001/api";
        
        public TaskServiceApp(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        public async Task<IEnumerable<TaskDto>> LoadActualTasksAsync(string userId, DateTime from)
        {
            var url = $"{Url}/tasks?from={DateToString(from)}&userId={userId}";
            var response = await (await HttpClient).GetAsync(url);
            return await ParseBodyAsync<IEnumerable<TaskDto>>(response);
        }

        public async Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to)
        {
            var url = $"{Url}/tasks/byDate?from={DateToString(from)}&to={DateToString(to)}&userId={userId}";
            var response = await (await HttpClient).GetAsync(url);
            return await ParseBodyAsync<IEnumerable<TaskDto>>(response);
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId)
        {
            var url = $"{Url}/tasks?userId={userId}";
            var response = await (await HttpClient).PostAsync(url, SerializePayload(tasks));
            return await ParseBodyAsync<IEnumerable<TaskDto>>(response);
        }

        public async Task<int> CreateRecurrencesAsync(int timezoneOffset, string userId)
        {
            var url = $"{Url}/recurrences/create?timezoneOffset={timezoneOffset}&userId={userId}";
            var response = await (await HttpClient).PostAsync(url, null);
            return await ParseBodyAsync<int>(response);
        }

        public async Task<IEnumerable<PlannedRecurrenceDto>> LoadRecurrencesAsync(string userId)
        {
            var url = $"{Url}/recurrences?userId={userId}";
            var response = await (await HttpClient).GetAsync(url);
            return await ParseBodyAsync<IEnumerable<PlannedRecurrenceDto>>(response);
        }

        public async Task<int> SaveRecurrencesAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId)
        {
            var url = $"{Url}/recurrences?userId={userId}";
            var response = await (await HttpClient).PostAsync(url, SerializePayload(recurrences));
            return await ParseBodyAsync<int>(response);
        }

        public async Task<TaskDto> ParseTask(string text)
        {
            var url = $"{Url}/parser?text={text}";
            var response = await (await HttpClient).GetAsync(url);
            return await ParseBodyAsync<TaskDto>(response);
        }

        public async Task<string> PrintTasks(IEnumerable<TaskDto> tasks)
        {
            var url = $"{Url}/parser/print";
            var response = await (await HttpClient).PostAsync(url, SerializePayload(tasks));
            return await ParseBodyAsync<string>(response);
        }
    }
}