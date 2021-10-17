using System;
using System.Collections.Generic;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Tasks.LoadActual
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