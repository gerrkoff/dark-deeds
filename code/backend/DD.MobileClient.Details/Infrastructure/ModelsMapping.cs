using AutoMapper;
using TaskDtoMobileClient = DD.MobileClient.Domain.Infrastructure.Dto.TaskDto;
using TaskDtoTaskService = DD.ServiceTask.Domain.Dto.TaskDto;

namespace DD.MobileClient.Details.Infrastructure;

internal sealed class ModelsMapping : Profile
{
    public ModelsMapping()
    {
        CreateMap<TaskDtoMobileClient, TaskDtoTaskService>().ReverseMap();
    }
}
