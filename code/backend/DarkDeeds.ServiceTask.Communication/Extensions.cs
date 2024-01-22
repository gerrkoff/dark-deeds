using DarkDeeds.Communication;
using DarkDeeds.ServiceTask.Communication.Publishers;
using DD.TaskService.Domain.Dto;
using DD.TaskService.Domain.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceTask.Communication;

public static class Extensions
{
    public static void AddTaskCommunication(this IServiceCollection services)
    {
        services.AddDarkDeedsAmpqPublisher<ITaskUpdatedPublisher, TaskUpdatedPublisher, TaskUpdatedDto>();
        services.AddScoped<INotifierService, NotifierService>();
    }
}
