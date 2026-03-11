using MediatR;
using RestaurantOrderTracking.Application.Feature.OrderItem.Commands.Create;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.CreateOnline
{
    /// <summary>
    /// Command để khách hàng đặt hàng online tại nhà.
    /// Tạo Customer mới, Order (TableId=null, OrderType=Delivery, Status=Pending)
    /// và các OrderItem trong một request duy nhất.
    /// OrderChannel sẽ tự động được gán là "Online".
    /// </summary>
    public record CreateOnlineOrderCommand(
        // Thông tin khách hàng
        string CustomerName,
        string CustomerPhone,
        string CustomerAddress,

        // Danh sách sản phẩm — dùng chung record OrderItemEntry
        List<OrderItemEntry> Items
    ) : IRequest<Result<Guid>>;
}
