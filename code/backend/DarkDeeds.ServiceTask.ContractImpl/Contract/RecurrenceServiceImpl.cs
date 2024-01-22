using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Authentication.Core;
using DarkDeeds.ServiceTask.Contract;
using DD.TaskService.Domain.Dto;
using DD.TaskService.Domain.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using RecurrenceService = DarkDeeds.ServiceTask.Contract.RecurrenceService;

namespace DarkDeeds.ServiceTask.ContractImpl.Contract;

[Authorize]
public class RecurrenceServiceImpl : RecurrenceService.RecurrenceServiceBase
{
    private readonly IMapper _mapper;
    private readonly IRecurrenceService _recurrenceService;
    private readonly IRecurrenceCreatorService _recurrenceCreatorService;

    public RecurrenceServiceImpl(IMapper mapper, IRecurrenceService recurrenceService, IRecurrenceCreatorService recurrenceCreatorService)
    {
        _mapper = mapper;
        _recurrenceService = recurrenceService;
        _recurrenceCreatorService = recurrenceCreatorService;
    }

    public override async Task<CreateTasksReply> CreateTasks(CreateTasksRequest request, ServerCallContext context)
    {
        var authToken = context.GetHttpContext().User.ToAuthToken();
        var result = await _recurrenceCreatorService.CreateAsync(request.TimezoneOffset, authToken.UserId);
        return new CreateTasksReply
        {
            TasksCreatedCount = result
        };
    }

    public override async Task<LoadRecurrencesReply> LoadRecurrences(LoadRecurrencesRequest request, ServerCallContext context)
    {
        var authToken = context.GetHttpContext().User.ToAuthToken();
        var result = await _recurrenceService.LoadAsync(authToken.UserId);
        return new LoadRecurrencesReply
        {
            Recurrences = {_mapper.Map<IEnumerable<PlannedRecurrenceModel>>(result)}
        };
    }

    public override async Task<SaveRecurrencesReply> SaveRecurrences(SaveRecurrencesRequest request, ServerCallContext context)
    {
        var authToken = context.GetHttpContext().User.ToAuthToken();
        var recurrences = _mapper.Map<ICollection<PlannedRecurrenceDto>>(request.Recurrences);
        var result = await _recurrenceService.SaveAsync(recurrences, authToken.UserId);
        return new SaveRecurrencesReply
        {
            RecurrencesUpdatedCount = result
        };
    }
}
