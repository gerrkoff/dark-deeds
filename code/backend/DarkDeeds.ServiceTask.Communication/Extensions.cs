using DarkDeeds.Communication;
using DarkDeeds.ServiceTask.Communication.Publishers;
using DarkDeeds.ServiceTask.Infrastructure.Services;
using DarkDeeds.ServiceTask.Infrastructure.Services.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceTask.Communication
{
    public static class Extensions
    {   
        public static void AddTaskCommunication(this IServiceCollection services)
        {
            services.AddDarkDeedsAmpqPublisher<ITaskUpdatedPublisher, TaskUpdatedPublisher, TaskUpdatedDto>();
            services.AddScoped<INotifierService, NotifierService>();
        }
    }
}