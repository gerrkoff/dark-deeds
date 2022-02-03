using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Communication.Services.Interface;
using DarkDeeds.ServiceTask.Contract;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using Grpc.Core;

namespace DarkDeeds.WebClientBffApp.Communication.Apps
{
    public class TaskServiceApp : ITaskServiceApp
    {
        private readonly IMapper _mapper;
        private readonly IDdGrpcClientFactory<TaskService.TaskServiceClient> _taskClientFactory;
        private readonly IDdGrpcClientFactory<RecurrenceService.RecurrenceServiceClient> _recurrenceClientFactory;

        public TaskServiceApp(IMapper mapper,
            IDdGrpcClientFactory<TaskService.TaskServiceClient> taskClientFactory,
            IDdGrpcClientFactory<RecurrenceService.RecurrenceServiceClient> recurrenceClientFactory)
        {
            _mapper = mapper;
            _taskClientFactory = taskClientFactory;
            _recurrenceClientFactory = recurrenceClientFactory;
        }


        public async Task<IEnumerable<TaskDto>> LoadActualTasksAsync(DateTime from)
        {
            var request = new LoadActualRequest
            {
                FromDate = from.Ticks
            };
            var client = await _taskClientFactory.Create();
            var response = await client.LoadActualAsync(request, new Metadata());
            return _mapper.Map<IEnumerable<TaskDto>>(response.Tasks);
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks)
        {
            var request = new SaveRequest
            {
                Tasks = {_mapper.Map<IEnumerable<TaskModel>>(tasks)}
            };
            var client = await _taskClientFactory.Create();
            var response = await client.SaveAsync(request, new Metadata());
            return _mapper.Map<IEnumerable<TaskDto>>(response.Tasks);
        }

        public async Task<int> CreateRecurrencesAsync(int timezoneOffset)
        {
            var request = new CreateTasksRequest
            {
                TimezoneOffset = timezoneOffset
            };
            var client = await _recurrenceClientFactory.Create();
            var response = await client.CreateTasksAsync(request, new Metadata());
            return response.TasksCreatedCount;
        }

        public async Task<IEnumerable<PlannedRecurrenceDto>> LoadRecurrencesAsync()
        {
            var request = new LoadRecurrencesRequest();
            var client = await _recurrenceClientFactory.Create();
            var response = await client.LoadRecurrencesAsync(request, new Metadata());
            return _mapper.Map<IEnumerable<PlannedRecurrenceDto>>(response.Recurrences);
        }

        public async Task<int> SaveRecurrencesAsync(ICollection<PlannedRecurrenceDto> recurrences)
        {
            var request = new SaveRecurrencesRequest
            {
                Recurrences = {_mapper.Map<IEnumerable<PlannedRecurrenceModel>>(recurrences)}
            };
            var client = await _recurrenceClientFactory.Create();
            var response = await client.SaveRecurrencesAsync(request, new Metadata());
            return response.RecurrencesUpdatedCount;
        }
    }
}