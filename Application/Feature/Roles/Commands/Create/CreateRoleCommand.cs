using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Roles.Commands.Create
{
    public record CreateRoleCommand(string Name, string Description) : IRequest<Result<int>>;
}