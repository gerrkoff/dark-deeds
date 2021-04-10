using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Authentication;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.Dto;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.GetCurrentUser
{
    public class GetCurrentUserRequestHandler : IRequestHandler<GetCurrentUserRequestModel, CurrentUserDto>
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCurrentUserRequestHandler(IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<CurrentUserDto> Handle(GetCurrentUserRequestModel request, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext.User;
            
            var currentUser = user.Identity != null && user.Identity.IsAuthenticated
                ? _mapper.Map<CurrentUserDto>(user.ToAuthToken())
                : new CurrentUserDto();

            return Task.FromResult(currentUser);
        }
    }
}