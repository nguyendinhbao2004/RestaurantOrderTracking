using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using OrderEntity = RestaurantOrderTracking.Domain.Entities.Order;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.CreateOnline
{
    public class CreateOnlineOrderHandler : IRequestHandler<CreateOnlineOrderCommand, Result<Guid>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOnlineOrderHandler(
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateOnlineOrderCommand request, CancellationToken cancellationToken)
        {
            // ===============================
            // 1️⃣ Validate danh sách items
            // ===============================
            if (request.Items == null || request.Items.Count == 0)
                return Result<Guid>.Failure("Items list cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.CustomerName))
                return Result<Guid>.Failure("Customer name is required.");

            if (string.IsNullOrWhiteSpace(request.CustomerPhone))
                return Result<Guid>.Failure("Customer phone is required.");

            if (string.IsNullOrWhiteSpace(request.CustomerAddress))
                return Result<Guid>.Failure("Customer address is required.");

            // ===============================
            // 2️⃣ Tạo Customer mới
            // ===============================
            var customer = new Customer(
                name: request.CustomerName,
                phone: request.CustomerPhone,
                address: request.CustomerAddress
            );

            await _customerRepository.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ===============================
            // 3️⃣ Tạo Order online
            //    TableId = null, OrderType = Delivery, Status = Pending
            // ===============================
            var order = new OrderEntity(
                orderType: OrderType.Delivery,
                customerId: customer.Id
            );

            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ===============================
            // 4️⃣ Thêm OrderItems vào Order
            // ===============================
            try
            {
                foreach (var item in request.Items)
                {
                    order.AddItem(
                        productId: item.ProductId,
                        accountId: Guid.Empty,   // Không có waiter/staff với đơn online
                        note: item.Note ?? string.Empty,
                        orderChannel: "Online"
                    );
                }
            }
            catch (InvalidOperationException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Online order created successfully.", order.Id);
        }
    }
}
