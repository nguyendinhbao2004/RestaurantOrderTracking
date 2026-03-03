using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.Bill.Commands.Cancel
{
    public class CancelBillHandler : IRequestHandler<CancelBillCommand, Result>
    {
        private readonly IBillRepository _billRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelBillHandler(
            IBillRepository billRepository,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _billRepository = billRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CancelBillCommand request, CancellationToken cancellationToken)
        {
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
                return Result.Failure("Bill not found.");

            if (bill.Status != BillStatus.unpaid)
                return Result.Failure("Only unpaid bills can be cancelled.");

            // Huỷ bill
            bill.Cancel();
            _billRepository.Update(bill, cancellationToken);

            // Trả Order về trạng thái Confirmed
            var order = await _orderRepository.GetByIdAsync(bill.OrderId, cancellationToken);
            if (order != null)
            {
                order.UpdateStatus(OrderStatus.Confirmed);
                _orderRepository.Update(order, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Bill cancelled successfully. Order reverted to Confirmed.");
        }
    }
}
