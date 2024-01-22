using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Authentication.Core;
using DarkDeeds.ServiceTask.Contract;
using DD.TaskService.Domain.Dto;
using DD.TaskService.Domain.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using TaskService = DarkDeeds.ServiceTask.Contract.TaskService;

namespace DarkDeeds.ServiceTask.ContractImpl.Contract;

[Authorize]
public class TaskServiceImpl : TaskService.TaskServiceBase
{
    private readonly IMapper _mapper;
    private readonly ITaskService _taskService;

    public TaskServiceImpl(IMapper mapper, ITaskService taskService)
    {
        _mapper = mapper;
        _taskService = taskService;
    }

    public override async Task<LoadActualReply> LoadActual(LoadActualRequest request, ServerCallContext context)
    {
        var authToken = context.GetHttpContext().User.ToAuthToken();
        var result = await _taskService.LoadActualTasksAsync(authToken.UserId,
            new DateTime(request.FromDate));
        return new LoadActualReply
        {
            Tasks = {_mapper.Map<IEnumerable<TaskModel>>(result)}
        };
    }

    public override async Task<LoadByDateReply> LoadByDate(LoadByDateRequest request, ServerCallContext context)
    {
        var authToken = context.GetHttpContext().User.ToAuthToken();
        var result = await _taskService.LoadTasksByDateAsync(authToken.UserId,
            new DateTime(request.FromDate),
            new DateTime(request.ToDate));
        return new LoadByDateReply
        {
            Tasks = {_mapper.Map<IEnumerable<TaskModel>>(result)}
        };
    }

    public override async Task<SaveReply> Save(SaveRequest request, ServerCallContext context)
    {
        var authToken = context.GetHttpContext().User.ToAuthToken();
        var tasks = _mapper.Map<ICollection<TaskDto>>(request.Tasks);
        var result = await _taskService.SaveTasksAsync(tasks, authToken.UserId);
        return new SaveReply
        {
            Tasks = {_mapper.Map<IEnumerable<TaskModel>>(result)}
        };
    }
}
