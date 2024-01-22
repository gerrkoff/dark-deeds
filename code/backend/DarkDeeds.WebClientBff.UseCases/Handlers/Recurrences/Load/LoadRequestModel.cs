using System.Collections.Generic;
using DD.TaskService.Domain.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Recurrences.Load;

public class LoadRequestModel : IRequest<IEnumerable<PlannedRecurrenceDto>>;
