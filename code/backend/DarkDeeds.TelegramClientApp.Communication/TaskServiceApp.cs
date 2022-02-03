using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Authentication.Core.Models;
using DarkDeeds.Authentication.Core.Services;
using DarkDeeds.Communication;
using DarkDeeds.Communication.Services.Interface;
using DarkDeeds.TaskServiceApp.Contract;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp.Dto;
using Grpc.Core;

namespace DarkDeeds.TelegramClientApp.Communication
{
    public class TaskServiceApp : ITaskServiceApp
    {
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IDdGrpcClientFactory<TaskService.TaskServiceClient> _taskClientFactory;
        private readonly IDdGrpcClientFactory<ParserService.ParserServiceClient> _parserClientFactory;
        
        public TaskServiceApp(IMapper mapper, IDdGrpcClientFactory<TaskService.TaskServiceClient> taskClientFactory, ITokenService tokenService, IDdGrpcClientFactory<ParserService.ParserServiceClient> parserClientFactory)
        {
            _mapper = mapper;
            _taskClientFactory = taskClientFactory;
            _tokenService = tokenService;
            _parserClientFactory = parserClientFactory;
        }

        public async Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to)
        {
            var client = await _taskClientFactory.Create();
            var metadata = CreateHeaders(userId);
            var reply = await client.LoadByDateAsync(new LoadByDateRequest
            {
                FromDate = from.Ticks,
                ToDate = to.Ticks
            }, metadata);
            return _mapper.Map<IEnumerable<TaskDto>>(reply.Tasks);
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId)
        {
            var client = await _taskClientFactory.Create();
            var metadata = CreateHeaders(userId);
            var reply = await client.SaveAsync(new SaveRequest
            {
                Tasks = {_mapper.Map<IEnumerable<TaskModel>>(tasks)}
            }, metadata);
            
            return _mapper.Map<IEnumerable<TaskDto>>(reply.Tasks);
        }

        public async Task<TaskDto> ParseTask(string text)
        {
            var client = await _parserClientFactory.Create();
            var reply = await client.ParseAsync(new ParseRequest
            {
                Text = text,
            });
            
            return _mapper.Map<TaskDto>(reply.Task);
        }

        public async Task<ICollection<string>> PrintTasks(IEnumerable<TaskDto> tasks)
        {
            var client = await _parserClientFactory.Create();
            var reply = await client.PrintAsync(new PrintRequest
            {
                Tasks = {_mapper.Map<IEnumerable<TaskModel>>(tasks)}
            });

            return reply.Texts;
        }

        private Metadata CreateHeaders(string userId)
        {
            var headers = new Metadata();
            
            var token = new AuthToken
            {
                UserId = userId,
                Username = "",
                DisplayName = "",
            };
            var tokenSerialized = _tokenService.Serialize(token);
            headers.AddAuthorizationIfEmpty(tokenSerialized);

            return headers;
        }
    }
}