using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Enums;
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
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IGenericRepository<QRSession> _qrSessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessWebhookHandler> _logger;

        public ProcessWebhookHandler(
            IPayOSService payOSService,
            IPaymentTransactionRepository paymentTransactionRepository,
            IBillRepository billRepository,
            IOrderRepository orderRepository,
            ITableRepository tableRepository,
            IGenericRepository<QRSession> qrSessionRepository,
            IUnitOfWork unitOfWork,
            ILogger<ProcessWebhookHandler> logger)
        {
            _payOSService = payOSService;
            _paymentTransactionRepository = paymentTransactionRepository;
            _billRepository = billRepository;
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _qrSessionRepository = qrSessionRepository;
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

            // update payment transaction status
            if (transaction.Status != "PAID")
            {
                transaction.UpdateStatus("PAID");
            }

            // update bill
            var bill = await _billRepository.GetByIdWithDetailsAsync(transaction.BillId);

            if (bill == null)
            {
                _logger.LogError("Webhook PayOS: Không tìm thấy bill với billId={BillId}", transaction.BillId);
                return Result<string>.Failure("Không tìm thấy hóa đơn trong hệ thống.");
            }

            if (bill.Status == BillStatus.unpaid)
            {
                // record payment method as bank transfer
                bill.Update(PaymentMethod.bank_transfer, bill.Discount);
                // record transactionId from PayOS and mark as paid
                bill.MarkAsPaid(webhookData.Reference);
            }

            var order = bill.Order ?? await _orderRepository.GetByIdAsync(bill.OrderId, cancellationToken);
            if (order != null)
            {
                var isOrderCompleted = order.Status == OrderStatus.Completed;

                if (order.Status != OrderStatus.Completed && order.Status != OrderStatus.Cancelled)
                {
                    var canCompleteOrder = order.Status == OrderStatus.Paying ||
                        (order.OrderTypes == OrderType.Delivery && order.Status == OrderStatus.Delivering);

                    if (canCompleteOrder)
                    {
                        order.UpdateStatus(OrderStatus.Completed);
                        _orderRepository.Update(order, cancellationToken);
                        isOrderCompleted = true;
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Webhook PayOS: Không thể chuyển OrderId={OrderId} từ trạng thái {Status} sang Completed.",
                            order.Id,
                            order.Status);
                    }
                }

                if (isOrderCompleted && order.OrderTypes == OrderType.DineIn && order.TableId.HasValue)
                {
                    var table = await _tableRepository.GetByIdAsync(order.TableId.Value);
                    if (table != null)
                    {
                        table.SetAvailable();
                        _tableRepository.Update(table, cancellationToken);

                        var oldSessions = await _qrSessionRepository.FindAsync(
                            s => s.TableId == table.Id && s.IsActive);

                        foreach (var session in oldSessions)
                        {
                            session.Revoke();
                            _qrSessionRepository.Update(session, cancellationToken);
                        }

                        var newSession = new QRSession(table.Id);
                        await _qrSessionRepository.AddAsync(newSession);
                    }
                }
            }

            // Lưu vào DB
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("PayOS Webhook xử lý thành công. OrderCode={Code}, Amount={Amount}",
                webhookData.OrderCode, webhookData.Amount);

            return Result<string>.Success("Xử lý webhook thành công.");
        }
    }
}
