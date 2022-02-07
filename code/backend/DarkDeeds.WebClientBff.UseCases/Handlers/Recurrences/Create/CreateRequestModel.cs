using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Recurrences.Create
{
    public class CreateRequestModel : IRequest<int>
    {
        public CreateRequestModel(int timezoneOffset)
        {
            TimezoneOffset = timezoneOffset;
        }

        public int TimezoneOffset { get; }
    }
}