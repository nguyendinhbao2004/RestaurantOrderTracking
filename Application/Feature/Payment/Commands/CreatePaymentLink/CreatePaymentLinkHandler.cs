using MediatR;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
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
            // Lấy và kiểm tra Bill
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
                return Result<PaymentLinkResponse>.Failure("Không tìm thấy hóa đơn.");

            if (bill.Status != Domain.Enums.BillStatus.unpaid)
                return Result<PaymentLinkResponse>.Failure("Hóa đơn đã được thanh toán hoặc đã bị hủy.");

            // Tạo orderCode duy nhất
            string timeString = DateTime.Now.ToString("yyMMddHHmmssfff");
            long orderCode = long.Parse(timeString);

            // Tạo mô tả thanh toán (tối đa 25 ký tự)
            string description = $"Bill-{bill.Id.ToString()[..8]}";
            if (description.Length > 25)
                description = description[..25];

            // Gọi PayOS API tạo link
            PaymentLinkResponse paymentLinkData;
            try
            {
                paymentLinkData = await _payOSService.CreatePaymentLinkAsync(
                    orderCode,
                    (int)bill.Amount,
                    description,
                    request.CancelUrl,
                    request.ReturnUrl);
            }
            catch (Exception ex)
            {
                return Result<PaymentLinkResponse>.Failure($"Tạo link thanh toán thất bại: {ex.Message}");
            }

            // Lưu PaymentTransaction vào database
            var transaction = new PaymentTransaction(
                billId: bill.Id,
                orderCode: orderCode,
                amount: bill.Amount,
                status: "PENDING");

            // Lưu PaymentLinkId để dùng cho GetInfo và Cancel
            transaction.SetPaymentLinkId(paymentLinkData.PaymentLinkId);

            await _paymentTransactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<PaymentLinkResponse>.Success("Tạo link thanh toán thành công.", paymentLinkData);
        }
    }
}
