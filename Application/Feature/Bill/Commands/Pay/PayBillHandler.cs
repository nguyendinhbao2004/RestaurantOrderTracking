using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.Bill.Commands.Pay
{
    public class PayBillHandler : IRequestHandler<PayBillCommand, Result>
    {
        private readonly IBillRepository _billRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IGenericRepository<QRSession> _qrSessionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PayBillHandler(
            IBillRepository billRepository,
            IOrderRepository orderRepository,
            ITableRepository tableRepository,
            IGenericRepository<QRSession> qrSessionRepository,
            IUnitOfWork unitOfWork)
        {
            _billRepository = billRepository;
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _qrSessionRepository = qrSessionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(PayBillCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm bill
            var bill = await _billRepository.GetByIdWithDetailsAsync(request.BillId);
            if (bill == null)
                return Result.Failure("Bill not found.");

            // 2. Validate trạng thái
            if (bill.Status != BillStatus.unpaid)
                return Result.Failure("Bill is not in unpaid status.");

            // 3. Đánh dấu đã thanh toán
            bill.MarkAsPaid();
            _billRepository.Update(bill, cancellationToken);

            // 4. Cập nhật Order → Completed
            var order = await _orderRepository.GetByIdAsync(bill.OrderId, cancellationToken);
            if (order != null)
            {
                order.UpdateStatus(OrderStatus.Completed);
                _orderRepository.Update(order, cancellationToken);

                // 5. Cập nhật Table → Available (cho DineIn)
                if (order.OrderTypes == OrderType.DineIn)
                {
                    var table = await _tableRepository.GetByIdAsync(order.TableId);
                    if (table != null)
                    {
                        table.SetAvailable();
                        _tableRepository.Update(table, cancellationToken);

                        // 6. Tự động refresh QR Session cho bàn
                        // Revoke tất cả session cũ
                        var oldSessions = await _qrSessionRepository.FindAsync(
                            s => s.TableId == table.Id && s.IsActive);
                        foreach (var session in oldSessions)
                        {
                            session.Revoke();
                            _qrSessionRepository.Update(session, cancellationToken);
                        }

                        // Tạo session mới (QR code vật lý giữ nguyên, chỉ đổi session token)
                        var newSession = new QRSession(table.Id);
                        await _qrSessionRepository.AddAsync(newSession);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Bill paid successfully. Order completed. QR session refreshed.");
        }
    }
}
