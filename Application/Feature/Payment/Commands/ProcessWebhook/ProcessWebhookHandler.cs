using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Domain.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Feature.Payment.Commands.ProcessWebhook
{

    public class ProcessWebhookHandler : IRequestHandler<ProcessWebhookCommand, Result<string>>
    {
        private readonly IPayOSService _payOSService;
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IBillRepository _billRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessWebhookHandler> _logger;

        public ProcessWebhookHandler(
            IPayOSService payOSService,
            IPaymentTransactionRepository paymentTransactionRepository,
            IBillRepository billRepository,
            IUnitOfWork unitOfWork,
            ILogger<ProcessWebhookHandler> logger)
        {
            _payOSService = payOSService;
            _paymentTransactionRepository = paymentTransactionRepository;
            _billRepository = billRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            // verify signature
            var webhookData = _payOSService.VerifyAndExtractWebhookData(request.Payload);

            if (webhookData == null)
            {
                _logger.LogWarning("PayOS Webhook bị từ chối do chữ ký không hợp lệ hoặc giao dịch thất bại.");
                return Result<string>.Failure("Chữ ký webhook không hợp lệ hoặc giao dịch không thành công.");
            }

            // find payment transaction in DB
            var transaction = await _paymentTransactionRepository.GetByOrderCodeAsync(webhookData.OrderCode);
            if (transaction == null)
            {
                _logger.LogError("Webhook PayOS: Không tìm thấy transaction với orderCode={OrderCode}", webhookData.OrderCode);
                return Result<string>.Failure("Không tìm thấy giao dịch trong hệ thống.");
            }

            if (transaction.Status == "PAID")
            {
                _logger.LogInformation("Webhook PayOS: OrderCode={OrderCode} đã được xử lý, bỏ qua.", webhookData.OrderCode);
                return Result<string>.Success("Giao dịch đã được xử lý trước đó.");
            }

            // update payment transaction status
            transaction.UpdateStatus("PAID");

            // update bill
            var bill = transaction.Bill
                       ?? await _billRepository.GetByIdAsync(transaction.BillId, cancellationToken);

            if (bill != null && bill.Status == Domain.Enums.BillStatus.unpaid)
            {
                // record payment method as bank transfer
                bill.Update(Domain.Enums.PaymentMethod.bank_transfer, bill.Discount);
                // record transactionId from PayOS and mark as paid
                bill.MarkAsPaid(webhookData.Reference);
            }

            // Lưu vào DB
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("PayOS Webhook xử lý thành công. OrderCode={Code}, Amount={Amount}",
                webhookData.OrderCode, webhookData.Amount);

            return Result<string>.Success("Xử lý webhook thành công.");
        }
    }
}
