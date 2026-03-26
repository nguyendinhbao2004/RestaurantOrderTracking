using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;

namespace RestaurantOrderTracking.Application.Feature.OrderItem.Commands.UpdateStatus
{
    /// <summary>
    /// The JSON body fields for updating an OrderItem status.
    /// OrderItemId comes from the route parameter, not the body.
    /// AccountId is nullable — null means the action was performed by a customer.
    /// </summary>
    public record UpdateStatusOrderItemRequest(
        List<Guid> OrderItemIds,
        OrderItemStatus NewStatus,
        Guid? AccountId,
        string ChangeSource,
        Guid? AssigneeId = null,
        string? Note = null
    );

    /// <summary>
    /// Full MediatR command constructed by the controller (route param + body).
    /// - Confirmed(1) → Cooking(2): AssigneeId required → sets ChefAccountId.
    /// - Ready(3) → Delivering(4): AssigneeId auto-set from AccountId → sets WaiterAccountId.
    /// - Cancelled(6): only allowed from Pending(0) or Confirmed(1).
    /// - AccountId null = customer action, logged as null in OrderItemLog.
    /// - All transitions write an OrderItemLog entry.
    /// </summary>
    public record UpdateStatusOrderItemCommand(
        List<Guid> OrderItemIds,
        OrderItemStatus NewStatus,
        Guid? AccountId,
        string ChangeSource,
        Guid? AssigneeId = null,
        string? Note = null
    ) : IRequest<Result>;
}
