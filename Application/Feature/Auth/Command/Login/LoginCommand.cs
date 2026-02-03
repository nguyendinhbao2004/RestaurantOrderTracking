using MediatR;
using RestaurantOrderTracking.Application.Dto.Auth;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Auth.Command.Login
{
    public class LoginCommand : IRequest<Result<AuthResponse>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
