using System.Collections.Generic;
using DarkDeeds.WebClientBff.Infrastructure.Communication.TaskServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Recurrences.Load
{
    public class LoadRequestModel : IRequest<IEnumerable<PlannedRecurrenceDto>>
    {
    }
}