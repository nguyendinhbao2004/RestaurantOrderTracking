using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.CreateOnline
{
    public record OnlineOrderItemEntry(Guid ProductId, string? Note, int Quantity);
    public record CreateOnlineOrderCommand(
        string CustomerName,
        string CustomerPhone,
        string CustomerAddress,
        PaymentMethod PaymentMethod,

        // Danh sách sản phẩm 
        List<OnlineOrderItemEntry> Items
    ) : IRequest<Result<CreateOnlineOrderResponse>>;
}
