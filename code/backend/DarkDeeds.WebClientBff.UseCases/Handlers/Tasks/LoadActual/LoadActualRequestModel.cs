using System;
using System.Collections.Generic;
using DarkDeeds.ServiceTask.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Tasks.LoadActual
{
    public class LoadActualRequestModel : IRequest<IEnumerable<TaskDto>>
    {
        public LoadActualRequestModel(DateTime from)
        {
            From = from;
        }

        public DateTime From { get; }
    }
}
