using MediatR;
using RestaurantOrderTracking.Application.Common.Interface;
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

        public ProcessWebhookHandler(
            IPayOSService payOSService,
            IPaymentTransactionRepository paymentTransactionRepository,
            IBillRepository billRepository,
            IUnitOfWork unitOfWork)
        {
            _payOSService = payOSService;
            _paymentTransactionRepository = paymentTransactionRepository;
            _billRepository = billRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            var verifiedOrderCode = await _payOSService.VerifyPaymentWebhook(request.WebhookBody);

            if (verifiedOrderCode == null)
            {
                return Result<string>.Failure("Webhook signature verification failed or payment not successful.");
            }

            // Find transaction
            var transaction = await _paymentTransactionRepository.GetByOrderCodeAsync(verifiedOrderCode.Value);
            if (transaction == null)
            {
                return Result<string>.Failure("Payment transaction not found in database.");
            }

            if (transaction.Status == "PAID")
            {
                // Already paid, ignore to be idempotent
                return Result<string>.Success("Already processed.");
            }

            // Update Transaction
            transaction.UpdateStatus("PAID");

            // Update Bill
            var bill = transaction.Bill;
            if (bill != null && bill.Status == Domain.Enums.BillStatus.unpaid)
            {
                bill.MarkAsPaid(transaction.OrderCode.ToString());
                bill.Update(Domain.Enums.PaymentMethod.bank_transfer, bill.Discount);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Webhook processed successfully.");
        }
    }
}
