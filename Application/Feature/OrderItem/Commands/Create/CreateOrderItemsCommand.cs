using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Commands.Create
{
    /// <summary>
    /// Represents a single product entry when creating order items.
    /// </summary>
    public record OrderItemEntry(Guid ProductId, string? Note, int Quantity);

    /// <summary>
    /// Command to create multiple order items for a single order in one request.
    /// The shared fields (OrderId, OrderChannel, CreatedBy) apply to every item in the list.
    /// </summary>
    public record CreateOrderItemsCommand(
        Guid OrderId,
        string OrderChannel,
        Guid? CreatedBy,
        List<OrderItemEntry> Items
    ) : IRequest<Result>;
}
