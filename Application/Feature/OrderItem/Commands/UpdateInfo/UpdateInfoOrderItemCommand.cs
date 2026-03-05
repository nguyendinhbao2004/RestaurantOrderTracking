using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Commands.UpdateInfo
{
    /// <summary>
    /// JSON body for updating OrderItem info fields.
    /// OrderItemId comes from the route parameter.
    /// Pass null to skip updating a field.
    /// </summary>
    public record UpdateInfoOrderItemRequest(
        Guid? ChefAccountId,
        Guid? WaiterAccountId,
        string? Note
    );

    /// <summary>
    /// Full MediatR command assembled by the controller.
    /// Each field is conditionally validated against the current OrderItem status in the handler.
    /// </summary>
    public record UpdateInfoOrderItemCommand(
        Guid OrderItemId,
        Guid? ChefAccountId,
        Guid? WaiterAccountId,
        string? Note
    ) : IRequest<Result>;
}
