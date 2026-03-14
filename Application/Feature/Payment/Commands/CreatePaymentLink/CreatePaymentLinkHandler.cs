using MediatR;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Domain.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Feature.Payment.Commands.CreatePaymentLink
{
    public class CreatePaymentLinkHandler : IRequestHandler<CreatePaymentLinkCommand, Result<string>>
    {
        private readonly IBillRepository _billRepository;
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IPayOSService _payOSService;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePaymentLinkHandler(
            IBillRepository billRepository,
            IPaymentTransactionRepository paymentTransactionRepository,
            IPayOSService payOSService,
            IUnitOfWork unitOfWork)
        {
            _billRepository = billRepository;
            _paymentTransactionRepository = paymentTransactionRepository;
            _payOSService = payOSService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(CreatePaymentLinkCommand request, CancellationToken cancellationToken)
        {
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
            {
                return Result<string>.Failure("Bill not found.");
            }

            if (bill.Status != Domain.Enums.BillStatus.unpaid)
            {
                return Result<string>.Failure("Bill is already paid or cancelled.");
            }

            // Generate unique OrderCode (up to 53 bits for PayOS). Using yyMMddHHmmssfff (15 digits)
            string timeString = DateTime.Now.ToString("yyMMddHHmmssfff");
            long orderCode = long.Parse(timeString);

            // Create PaymentTransaction
            var paymentTransaction = new PaymentTransaction(
                billId: bill.Id,
                orderCode: orderCode,
                amount: bill.Amount,
                status: "PENDING"
            );

            // Call PayOS Service
            string description = $"Pay for bill {bill.Id.ToString().Substring(0, 8)}";
            // PayOS desc has limit on chars (25 chars max). So truncate it.
            if (description.Length > 25) description = description.Substring(0, 25);

            string checkoutUrl;
            try
            {
                checkoutUrl = await _payOSService.CreatePaymentLink(orderCode, (int)bill.Amount, description, request.CancelUrl, request.ReturnUrl);
                paymentTransaction.SetPaymentLinkId(checkoutUrl);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Failed to create payment link: {ex.Message}");
            }

            await _paymentTransactionRepository.AddAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Payment link created successfully", checkoutUrl);
        }
    }
}
