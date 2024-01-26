using AutoMapper;
using TaskDtoTelegramClient = DD.TelegramClient.Domain.Infrastructure.Dto.TaskDto;
using TaskDtoTaskService = DD.ServiceTask.Domain.Dto.TaskDto;

namespace DD.TelegramClient.Details.Infrastructure;

class ModelsMapping : Profile
{
    public ModelsMapping()
    {
        CreateMap<TaskDtoTelegramClient, TaskDtoTaskService>().ReverseMap();
    }
}
