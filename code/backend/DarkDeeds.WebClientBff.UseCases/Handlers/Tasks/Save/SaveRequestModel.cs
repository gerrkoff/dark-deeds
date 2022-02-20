using System.Collections.Generic;
using DarkDeeds.ServiceTask.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Tasks.Save
{
    public class SaveRequestModel : IRequest<IEnumerable<TaskDto>>
    {
        public SaveRequestModel(ICollection<TaskDto> tasks)
        {
            Tasks = tasks;
        }

        public ICollection<TaskDto> Tasks { get; }
    }
}
