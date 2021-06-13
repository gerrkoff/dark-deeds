using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// using AutoMapper;
// using DarkDeeds.Communication.Services.Interface;
// using DarkDeeds.TaskServiceApp.Contract;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp.Dto;
// using Grpc.Core;

namespace DarkDeeds.TelegramClientApp.Communication
{
    public class TaskServiceApp : ITaskServiceApp
    {
        // private readonly IMapper _mapper;
        // private readonly IDdGrpcClientFactory<TaskService.TaskServiceClient> _clientFactory;
        //
        // public TaskServiceApp(IMapper mapper, IDdGrpcClientFactory<TaskService.TaskServiceClient> clientFactory)
        // {
        //     _mapper = mapper;
        //     _clientFactory = clientFactory;
        // }

        public Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to)
        {
            // var client = await _clientFactory.Create();
            // var reply = await client.LoadByDateAsync(new LoadByDateRequest
            // {
            //     FromDate = from.Ticks,
            //     ToDate = to.Ticks
            // }, AuthMetadata(userId));
            // return _mapper.Map<IEnumerable<TaskDto>>(reply);
            
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId)
        {
            // var url = $"{_url}/tasks?userId={userId}";
            // var response = await HttpClient.PostAsync(url, SerializePayload(tasks));
            // return await ParseBodyAsync<IEnumerable<TaskDto>>(response);
            
            throw new NotImplementedException();
        }

        public Task<TaskDto> ParseTask(string text)
        {
            // var url = $"{_url}/parser?text={text}";
            // var response = await HttpClient.GetAsync(url);
            // return await ParseBodyAsync<TaskDto>(response);
            
            throw new NotImplementedException();
        }

        public Task<string> PrintTasks(IEnumerable<TaskDto> tasks)
        {
            // var url = $"{_url}/parser/print";
            // var response = await HttpClient.PostAsync(url, SerializePayload(tasks));
            // return await ParseBodyAsync<string>(response);
            
            throw new NotImplementedException();
        }

        // private Metadata AuthMetadata(string userId) => new Metadata {{"UserId", userId}};
    }
}