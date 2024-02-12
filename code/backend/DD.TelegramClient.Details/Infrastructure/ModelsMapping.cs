using AutoMapper;
using TaskDtoTaskService = DD.ServiceTask.Domain.Dto.TaskDto;
using TaskDtoTelegramClient = DD.TelegramClient.Domain.Infrastructure.Dto.TaskDto;

namespace DD.TelegramClient.Details.Infrastructure;

internal sealed class ModelsMapping : Profile
{
    public ModelsMapping()
    {
        CreateMap<TaskDtoTelegramClient, TaskDtoTaskService>().ReverseMap();
    }
}
