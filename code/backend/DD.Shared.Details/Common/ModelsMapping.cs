using AutoMapper;
using DD.Shared.Details.Abstractions.Dto;
using TaskDtoTaskService = DD.ServiceTask.Domain.Dto.TaskDto;
using TaskUpdatedDtoTaskService = DD.ServiceTask.Domain.Dto.TaskUpdatedDto;

namespace DD.Shared.Details.Common;

internal sealed class ModelsMapping : Profile
{
    public ModelsMapping()
    {
        CreateMap<TaskDto, TaskDtoTaskService>().ReverseMap();
        CreateMap<TasksUpdatedDto, TaskUpdatedDtoTaskService>().ReverseMap();
    }
}
