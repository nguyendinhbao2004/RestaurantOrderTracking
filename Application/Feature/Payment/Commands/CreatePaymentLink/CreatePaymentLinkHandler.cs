using MediatR;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface.Repository;
using RestaurantOrderTracking.Domain.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Feature.Payment.Commands.CreatePaymentLink
{
    public class CreatePaymentLinkHandler : IRequestHandler<CreatePaymentLinkCommand, Result<PaymentLinkResponse>>
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

        public async Task<Result<PaymentLinkResponse>> Handle(CreatePaymentLinkCommand request, CancellationToken cancellationToken)
        {
            // get and check bill
            var bill = await _billRepository.GetByIdWithDetailsAsync(request.BillId);
            if (bill == null)
                return Result<PaymentLinkResponse>.Failure("Không tìm thấy hóa đơn.");

            if (bill.Status != Domain.Enums.BillStatus.unpaid)
                return Result<PaymentLinkResponse>.Failure("Hóa đơn đã được thanh toán hoặc đã bị hủy.");

            if (bill.Order == null)
                return Result<PaymentLinkResponse>.Failure("Không tìm thấy đơn hàng của hóa đơn.");

            if (bill.Order.OrderTypes == OrderType.Delivery)
            {
                if (!request.PayerAccountId.HasValue || request.PayerAccountId.Value == Guid.Empty)
                {
                    return Result<PaymentLinkResponse>.Failure(
                        "Đơn online yêu cầu người dùng đăng nhập để tạo link thanh toán.");
                }

                try
                {
                    bill.AssignAccount(request.PayerAccountId.Value);
                }
                catch (Exception ex)
                {
                    return Result<PaymentLinkResponse>.Failure(ex.Message);
                }
            }

            // create unique orderCode
            string timeString = DateTime.Now.ToString("yyMMddHHmmssfff");
            long orderCode = long.Parse(timeString);

            // create description (max 25 characters)
            string description = $"Bill-{bill.Id.ToString()[..8]}";
            if (description.Length > 25)
                description = description[..25];

            // call PayOS API to create payment link
            PaymentLinkResponse paymentLinkData;
            try
            {
                paymentLinkData = await _payOSService.CreatePaymentLinkAsync(
                    orderCode,
                    (int)bill.FinalAmount,
                    description,
                    request.CancelUrl,
                    request.ReturnUrl);
            }
            catch (Exception ex)
            {
                return Result<PaymentLinkResponse>.Failure($"Tạo link thanh toán thất bại: {ex.Message}");
            }

            // save payment transaction to database
            var transaction = new PaymentTransaction(
                billId: bill.Id,
                orderCode: orderCode,
                amount: bill.FinalAmount,
                status: "PENDING");

            // save payment link id for GetInfo and Cancel
            transaction.SetPaymentLinkId(paymentLinkData.PaymentLinkId);

            await _paymentTransactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<PaymentLinkResponse>.Success("Tạo link thanh toán thành công.", paymentLinkData);
        }
    }
}
