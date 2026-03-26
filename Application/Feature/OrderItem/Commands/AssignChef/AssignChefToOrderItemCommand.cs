using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Commands.AssignChef
{
    public record AssignChefToOrderItemRequest(Guid OrderItemId, Guid AccountId);

    public record AssignChefToOrderItemCommand(Guid OrderItemId, Guid AccountId) : IRequest<Result>;
}