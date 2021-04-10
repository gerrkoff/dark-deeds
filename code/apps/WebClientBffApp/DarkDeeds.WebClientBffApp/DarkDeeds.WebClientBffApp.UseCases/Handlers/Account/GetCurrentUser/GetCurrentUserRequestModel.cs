using DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.GetCurrentUser
{
    public class GetCurrentUserRequestModel : IRequest<CurrentUserDto>
    {
    }
}