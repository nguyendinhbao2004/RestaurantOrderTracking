using RestaurantOrderTracking.Domain.Enums;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.CreateOnline
{
    /// <summary>
    /// Kết quả trả về sau khi tạo đơn hàng online thành công.
    /// </summary>
    public record CreateOnlineOrderResponse(
        Guid OrderId,
        Guid BillId,
        PaymentMethod PaymentMethod
    );
}
