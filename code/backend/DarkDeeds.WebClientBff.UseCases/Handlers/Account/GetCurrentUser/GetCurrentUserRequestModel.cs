using DarkDeeds.WebClientBff.UseCases.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Account.GetCurrentUser
{
    public class GetCurrentUserRequestModel : IRequest<CurrentUserDto>
    {
    }
}