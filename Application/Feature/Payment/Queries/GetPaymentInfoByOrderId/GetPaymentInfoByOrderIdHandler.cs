using MediatR;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Feature.Payment.Queries.GetPaymentInfoByOrderId
{
    public class GetPaymentInfoByOrderIdHandler : IRequestHandler<GetPaymentInfoByOrderIdQuery, Result<PaymentInfoByOrderIdResponse>>
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;

        public GetPaymentInfoByOrderIdHandler(IPaymentTransactionRepository paymentTransactionRepository)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
        }

        public async Task<Result<PaymentInfoByOrderIdResponse>> Handle(GetPaymentInfoByOrderIdQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _paymentTransactionRepository.GetByOrderIdAsync(request.OrderId);
            if (transaction == null)
                return Result<PaymentInfoByOrderIdResponse>.Failure($"Không tìm thấy thông tin thanh toán cho orderId: {request.OrderId}.");

            var response = new PaymentInfoByOrderIdResponse
            {
                BillId = transaction.BillId,
                OrderCode = transaction.OrderCode,
                Amount = transaction.Amount,
                Status = transaction.Status,
                PaymentMetadata = transaction.PaymentMetadata == null
                    ? null
                    : new PaymentMetadataResponse
                    {
                        Bin = transaction.PaymentMetadata.Bin,
                        AccountNumber = transaction.PaymentMetadata.AccountNumber,
                        AccountName = transaction.PaymentMetadata.AccountName,
                        Description = transaction.PaymentMetadata.Description,
                        QrCode = transaction.PaymentMetadata.QrCode
                    }
            };

            return Result<PaymentInfoByOrderIdResponse>.Success("Lấy thông tin thanh toán thành công.", response);
        }
    }
}
