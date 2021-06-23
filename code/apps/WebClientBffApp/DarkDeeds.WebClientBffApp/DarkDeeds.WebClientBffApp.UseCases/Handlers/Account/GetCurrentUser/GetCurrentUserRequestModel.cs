using DarkDeeds.WebClientBffApp.UseCases.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.GetCurrentUser
{
    public class GetCurrentUserRequestModel : IRequest<CurrentUserDto>
    {
    }
}