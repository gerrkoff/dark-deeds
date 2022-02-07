using System.Collections.Generic;
using DarkDeeds.WebClientBff.Infrastructure.Communication.TaskServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Recurrences.Save
{
    public class SaveRequestModel : IRequest<int>
    {
        public SaveRequestModel(ICollection<PlannedRecurrenceDto> recurrences)
        {
            Recurrences = recurrences;
        }

        public ICollection<PlannedRecurrenceDto> Recurrences { get; }
    }
}