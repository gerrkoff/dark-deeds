using System.Collections.Generic;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Recurrences.Load
{
    public class LoadRequestModel : IRequest<IEnumerable<PlannedRecurrenceDto>>
    {
    }
}