using MediatR;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Payment.Queries.GetPaymentInfo
{
    /// <summary>
    /// Query lấy thông tin link thanh toán từ PayOS theo OrderCode.
    /// </summary>
    /// <param name="OrderCode">Mã orderCode đã lưu trong bảng PaymentTransaction.</param>
    public record GetPaymentInfoQuery(long OrderCode) : IRequest<Result<PaymentLinkInfoResponse>>;
}
