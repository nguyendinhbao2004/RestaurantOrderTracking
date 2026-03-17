using MediatR;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Domain.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Feature.Payment.Commands.CancelPaymentLink
{

    public class CancelPaymentLinkHandler : IRequestHandler<CancelPaymentLinkCommand, Result<CancelledPaymentLinkResponse>>
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IBillRepository _billRepository;
        private readonly IPayOSService _payOSService;
        private readonly IUnitOfWork _unitOfWork;

        public CancelPaymentLinkHandler(
            IPaymentTransactionRepository paymentTransactionRepository,
            IBillRepository billRepository,
            IPayOSService payOSService,
            IUnitOfWork unitOfWork)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
            _billRepository = billRepository;
            _payOSService = payOSService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CancelledPaymentLinkResponse>> Handle(CancelPaymentLinkCommand request, CancellationToken cancellationToken)
        {
            // Tìm PaymentTransaction theo OrderCode
            var transaction = await _paymentTransactionRepository.GetByOrderCodeAsync(request.OrderCode);
            if (transaction == null)
                return Result<CancelledPaymentLinkResponse>.Failure("Không tìm thấy giao dịch với mã order này.");

            if (transaction.Status == "PAID")
                return Result<CancelledPaymentLinkResponse>.Failure("Giao dịch đã được thanh toán, không thể hủy.");

            if (transaction.Status == "CANCELLED")
                return Result<CancelledPaymentLinkResponse>.Failure("Giao dịch đã được hủy trước đó.");

            if (string.IsNullOrWhiteSpace(transaction.PaymentLinkId))
                return Result<CancelledPaymentLinkResponse>.Failure("Giao dịch không có PaymentLinkId, không thể hủy qua PayOS.");

            // Gọi PayOS API hủy link
            CancelledPaymentLinkResponse cancelResult;
            try
            {
                cancelResult = await _payOSService.CancelPaymentLinkAsync(
                    transaction.PaymentLinkId,
                    request.CancellationReason);
            }
            catch (Exception ex)
            {
                return Result<CancelledPaymentLinkResponse>.Failure($"Hủy link trên PayOS thất bại: {ex.Message}");
            }

            // Cập nhật trạng thái trong database
            transaction.UpdateStatus("CANCELLED");

            // Hủy cả hóa đơn liên quan nếu còn unpaid
            var bill = await _billRepository.GetByIdAsync(transaction.BillId, cancellationToken);
            if (bill != null && bill.Status == Domain.Enums.BillStatus.unpaid)
                bill.Cancel();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CancelledPaymentLinkResponse>.Success("Hủy link thanh toán thành công.", cancelResult);
        }
    }
}
