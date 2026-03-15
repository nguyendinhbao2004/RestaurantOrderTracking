using MediatR;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.Payment.Commands.CancelPaymentLink
{
    /// <summary>
    /// Command yêu cầu hủy một link thanh toán PayOS.
    /// </summary>
    /// <param name="OrderCode">Mã orderCode nội bộ của giao dịch cần hủy.</param>
    /// <param name="CancellationReason">Lý do hủy (tùy chọn).</param>
    public record CancelPaymentLinkCommand(long OrderCode, string? CancellationReason = null)
     : IRequest<Result<CancelledPaymentLinkResponse>>;
}
