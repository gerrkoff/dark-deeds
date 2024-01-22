using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Authentication.Core.Models;
using DarkDeeds.Authentication.Core.Services;
using DarkDeeds.Communication;
using DarkDeeds.Communication.Services.Interface;
using DarkDeeds.ServiceTask.Contract;
using DD.TaskService.Domain.Dto;
using Grpc.Core;

namespace DarkDeeds.ServiceTask.Consumers.Impl;

class TaskServiceApp : ITaskServiceApp
{
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IDdGrpcClientFactory<TaskService.TaskServiceClient> _taskClientFactory;
    private readonly IDdGrpcClientFactory<RecurrenceService.RecurrenceServiceClient> _recurrenceClientFactory;
    private readonly IDdGrpcClientFactory<ParserService.ParserServiceClient> _parserClientFactory;

    public TaskServiceApp(IMapper mapper,
        ITokenService tokenService,
        IDdGrpcClientFactory<TaskService.TaskServiceClient> taskClientFactory,
        IDdGrpcClientFactory<RecurrenceService.RecurrenceServiceClient> recurrenceClientFactory,
        IDdGrpcClientFactory<ParserService.ParserServiceClient> parserClientFactory)
    {
        _mapper = mapper;
        _taskClientFactory = taskClientFactory;
        _recurrenceClientFactory = recurrenceClientFactory;
        _parserClientFactory = parserClientFactory;
        _tokenService = tokenService;
    }


    public async Task<IEnumerable<TaskDto>> LoadActualTasksAsync(DateTime from, string userId = null)
    {
        var request = new LoadActualRequest
        {
            FromDate = from.Ticks
        };
        var client = await _taskClientFactory.Create();
        var metadata = string.IsNullOrEmpty(userId) ? new Metadata() : CreateHeaders(userId);
        var reply = await client.LoadActualAsync(request, metadata);

        return _mapper.Map<IEnumerable<TaskDto>>(reply.Tasks);
    }

    public async Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(DateTime from, DateTime to, string userId = null)
    {
        var request = new LoadByDateRequest
        {
            FromDate = @from.Ticks,
            ToDate = to.Ticks
        };
        var client = await _taskClientFactory.Create();
        var metadata = string.IsNullOrEmpty(userId) ? new Metadata() : CreateHeaders(userId);
        var reply = await client.LoadByDateAsync(request, metadata);

        return _mapper.Map<IEnumerable<TaskDto>>(reply.Tasks);
    }

    public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId = null)
    {
        var request = new SaveRequest
        {
            Tasks = {_mapper.Map<IEnumerable<TaskModel>>(tasks)}
        };
        var client = await _taskClientFactory.Create();
        var metadata = string.IsNullOrEmpty(userId) ? new Metadata() : CreateHeaders(userId);
        var reply = await client.SaveAsync(request, metadata);

        return _mapper.Map<IEnumerable<TaskDto>>(reply.Tasks);
    }

    public async Task<int> CreateRecurrencesAsync(int timezoneOffset)
    {
        var request = new CreateTasksRequest
        {
            TimezoneOffset = timezoneOffset
        };
        var client = await _recurrenceClientFactory.Create();
        var reply = await client.CreateTasksAsync(request, new Metadata());

        return reply.TasksCreatedCount;
    }

    public async Task<IEnumerable<PlannedRecurrenceDto>> LoadRecurrencesAsync()
    {
        var request = new LoadRecurrencesRequest();
        var client = await _recurrenceClientFactory.Create();
        var reply = await client.LoadRecurrencesAsync(request, new Metadata());

        return _mapper.Map<IEnumerable<PlannedRecurrenceDto>>(reply.Recurrences);
    }

    public async Task<int> SaveRecurrencesAsync(ICollection<PlannedRecurrenceDto> recurrences)
    {
        var request = new SaveRecurrencesRequest
        {
            Recurrences = {_mapper.Map<IEnumerable<PlannedRecurrenceModel>>(recurrences)}
        };
        var client = await _recurrenceClientFactory.Create();
        var reply = await client.SaveRecurrencesAsync(request, new Metadata());

        return reply.RecurrencesUpdatedCount;
    }

    public async Task<TaskDto> ParseTask(string text)
    {
        var request = new ParseRequest
        {
            Text = text,
        };
        var client = await _parserClientFactory.Create();
        var reply = await client.ParseAsync(request);

        return _mapper.Map<TaskDto>(reply.Task);
    }

    public async Task<ICollection<string>> PrintTasks(IEnumerable<TaskDto> tasks)
    {
        var request = new PrintRequest
        {
            Tasks = {_mapper.Map<IEnumerable<TaskModel>>(tasks)}
        };
        var client = await _parserClientFactory.Create();
        var reply = await client.PrintAsync(request);

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
