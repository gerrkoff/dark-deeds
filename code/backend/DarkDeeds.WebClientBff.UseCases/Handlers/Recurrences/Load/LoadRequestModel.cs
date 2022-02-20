using System.Collections.Generic;
using DarkDeeds.ServiceTask.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Recurrences.Load
{
    public class LoadRequestModel : IRequest<IEnumerable<PlannedRecurrenceDto>>
    {
    }
}
