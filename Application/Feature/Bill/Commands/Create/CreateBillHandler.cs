using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using BillEntity = RestaurantOrderTracking.Domain.Entities.Bill;

namespace RestaurantOrderTracking.Application.Feature.Bill.Commands.Create
{
    public class CreateBillHandler : IRequestHandler<CreateBillCommand, Result<Guid>>
    {
        private readonly IBillRepository _billRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBillHandler(
            IBillRepository billRepository,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _billRepository = billRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateBillCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate: Order phải tồn tại
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result<Guid>.Failure("Order not found.");

            // 2. Validate: Order phải ở trạng thái Paying
            if (order.Status != Domain.Enums.OrderStatus.Paying)
                return Result<Guid>.Failure("Order must be in 'Paying' status to create a bill.");

            // 3. Validate: Order chưa có bill
            var existingBill = await _billRepository.GetByOrderIdAsync(request.OrderId);
            if (existingBill != null)
                return Result<Guid>.Failure("This order already has a bill.");

            // 4. Tính tổng tiền từ order items
            var amount = order.CalculateTotal();

            // 5. Tạo Bill
            var bill = new BillEntity(
                orderId: request.OrderId,
                accountId: request.CashierAccountId,
                amount: amount,
                paymentMethod: request.PaymentMethod,
                discount: request.Discount
            );

            await _billRepository.AddAsync(bill);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Bill created successfully.", bill.Id);
        }
    }
}
