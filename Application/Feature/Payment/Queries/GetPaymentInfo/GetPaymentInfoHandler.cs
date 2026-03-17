using MediatR;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Feature.Payment.Queries.GetPaymentInfo
{

    /// <summary>
    /// Handler xử lý query lấy thông tin link thanh toán PayOS.
    /// </summary>
    public class GetPaymentInfoHandler : IRequestHandler<GetPaymentInfoQuery, Result<PaymentLinkInfoResponse>>
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IPayOSService _payOSService;

        public GetPaymentInfoHandler(
            IPaymentTransactionRepository paymentTransactionRepository,
            IPayOSService payOSService)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
            _payOSService = payOSService;
        }

        public async Task<Result<PaymentLinkInfoResponse>> Handle(GetPaymentInfoQuery request, CancellationToken cancellationToken)
        {
            // Lấy PaymentTransaction để tìm PaymentLinkId
            var transaction = await _paymentTransactionRepository.GetByOrderCodeAsync(request.OrderCode);
            if (transaction == null)
                return Result<PaymentLinkInfoResponse>.Failure($"Không tìm thấy giao dịch với orderCode={request.OrderCode}.");

            if (string.IsNullOrWhiteSpace(transaction.PaymentLinkId))
                return Result<PaymentLinkInfoResponse>.Failure("Giao dịch chưa có PaymentLinkId, không thể truy vấn PayOS.");

            // Gọi PayOS API lấy thông tin link
            PaymentLinkInfoResponse info;
            try
            {
                info = await _payOSService.GetPaymentLinkInfoAsync(transaction.PaymentLinkId);
            }
            catch (System.Exception ex)
            {
                return Result<PaymentLinkInfoResponse>.Failure($"Lỗi khi lấy thông tin từ PayOS: {ex.Message}");
            }

            return Result<PaymentLinkInfoResponse>.Success("Lấy thông tin link thanh toán thành công.", info);
        }
    }
}
