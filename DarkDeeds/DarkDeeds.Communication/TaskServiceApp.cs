using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using DarkDeeds.Infrastructure.Communication;
using System.Text.Json;
using DarkDeeds.Infrastructure.Communication.Dto;

namespace DarkDeeds.Communication
{
    public class TaskServiceApp : ITaskServiceApp
    {
        private const string Url = "http://localhost:5001/api";
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public TaskServiceApp()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<TaskDto>> LoadActualTasksAsync(string userId, DateTime from)
        {
            var url = $"{Url}/tasks?from={DateToString(from)}&userId={userId}";
            var response = await _httpClient.GetAsync(url);
            return await ParseBodyAsync<IEnumerable<TaskDto>>(response);
        }

        public async Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to)
        {
            var url = $"{Url}/tasks/byDate?from={DateToString(from)}&to={DateToString(to)}&userId={userId}";
            var response = await _httpClient.GetAsync(url);
            return await ParseBodyAsync<IEnumerable<TaskDto>>(response);
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId)
        {
            var url = $"{Url}/tasks?userId={userId}";
            var response = await _httpClient.PostAsync(url, SerializePayload(tasks));
            return await ParseBodyAsync<IEnumerable<TaskDto>>(response);
        }

        public async Task<int> CreateRecurrencesAsync(int timezoneOffset, string userId)
        {
            var url = $"{Url}/recurrences/create?timezoneOffset={timezoneOffset}&userId={userId}";
            var response = await _httpClient.PostAsync(url, null);
            return await ParseBodyAsync<int>(response);
        }

        public async Task<IEnumerable<PlannedRecurrenceDto>> LoadRecurrencesAsync(string userId)
        {
            var url = $"{Url}/recurrences?userId={userId}";
            var response = await _httpClient.GetAsync(url);
            return await ParseBodyAsync<IEnumerable<PlannedRecurrenceDto>>(response);
        }

        public async Task<int> SaveRecurrencesAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId)
        {
            var url = $"{Url}/recurrences?userId={userId}";
            var response = await _httpClient.PostAsync(url, SerializePayload(recurrences));
            return await ParseBodyAsync<int>(response);
        }

        public async Task<TaskDto> ParseTask(string text)
        {
            var url = $"{Url}/parser?text={text}";
            var response = await _httpClient.GetAsync(url);
            return await ParseBodyAsync<TaskDto>(response);
        }

        public async Task<string> PrintTasks(IEnumerable<TaskDto> tasks)
        {
            var url = $"{Url}/parser/print";
            var response = await _httpClient.PostAsync(url, SerializePayload(tasks));
            return await ParseBodyAsync<string>(response);
        }

        private HttpContent SerializePayload<T>(T payload) =>
            new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8,
                MediaTypeNames.Application.Json);

        private async Task<T> ParseBodyAsync<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(body, _jsonOptions);
        }

        private string DateToString(DateTime dateTime) =>
            dateTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
    }
}