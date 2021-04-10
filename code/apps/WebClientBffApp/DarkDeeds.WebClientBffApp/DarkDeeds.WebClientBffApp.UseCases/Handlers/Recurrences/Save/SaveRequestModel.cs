using System.Collections.Generic;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Recurrences.Save
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